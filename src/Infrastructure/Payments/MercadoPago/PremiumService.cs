using Application.DTO.Premium;
using Application.Interfaces.Premium;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Infrastructure.Payments.MercadoPago;

public class PremiumService : IPremiumService
{
    private readonly MercadoPagoSettings _settings;
    private readonly IProfessionalProfileRepository _profiles;
    private readonly IUnitOfWork _uow;
    private readonly HttpClient _http;

    public PremiumService(IOptions<MercadoPagoSettings> settings,IProfessionalProfileRepository profiles,IUnitOfWork uow,HttpClient http)
    {
        _settings = settings.Value;
        _profiles = profiles;
        _uow = uow;
        _http = http;

        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.AccessToken);
    }

    public async Task<CreateSubscriptionResponse> CreateSubscriptionAsync(Guid professionalUserId, CancellationToken ct)
    {
        var profile = await _profiles.GetByUserIdAsync(professionalUserId, ct);

        if (profile is null)
            throw new InvalidOperationException("Perfil profissional não encontrado.");

        var requestBody = new
        {
            preapproval_plan_id = _settings.PlanId,
            payer_email = profile.User.Email.Address,
            back_url = _settings.SuccessUrl
        };

        var response = await _http.PostAsJsonAsync(
            "https://api.mercadopago.com/preapproval",
            requestBody,
            ct);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<MercadoPagoCreateSubscriptionResponse>(ct);

        if (json is null)
            throw new InvalidOperationException("Erro ao criar assinatura no Mercado Pago.");

        profile.MarkSubscriptionCreated(json.Id);

        await _uow.CommitAsync();

        return new CreateSubscriptionResponse(json.InitPoint, json.Id);
    }

    public async Task ProcessWebhookAsync(MercadoPagoWebhookData data, CancellationToken ct)
    {
        if (!string.Equals(data.Type, "preapproval", StringComparison.OrdinalIgnoreCase))
            return;

        var subscriptionId = data.Data.Id;

        var response = await _http.GetAsync(
            $"https://api.mercadopago.com/preapproval/{subscriptionId}", ct);

        response.EnsureSuccessStatusCode();

        var preapproval = await response.Content
            .ReadFromJsonAsync<MercadoPagoPreapprovalResponse>(cancellationToken: ct);

        if (preapproval is null)
            return;

        var profile = await _profiles.GetBySubscriptionIdAsync(preapproval.Id, ct);

        if (profile is null)
            return;

        var nextDate = preapproval.NextPaymentDate ?? DateTime.UtcNow.AddDays(30);

        switch (preapproval.Status?.ToLowerInvariant())
        {
            case "authorized":
                profile.ActivatePremium(preapproval.Id, nextDate);
                break;

            case "paused":
                profile.PausePremium();
                break;

            case "cancelled":
            case "cancelled_by_user":
            case "cancelled_by_merchant":
                profile.CancelPremium();
                break;

            default:
                profile.MarkChargeFailed();
                break;
        }

        await _uow.CommitAsync();
    }

    public async Task<PremiumStatusDto> GetStatusAsync(Guid userId, CancellationToken ct)
    {
        var profile = await _profiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (profile is null)
            throw new InvalidOperationException("Usuário não possui perfil profissional.");

        return new PremiumStatusDto(
            IsPremium: profile.IsPremium,
            Status: profile.SubscriptionStatus?.ToString() ?? "none",
            NextBillingDate: profile.NextBillingDate,
            ActivatedAt: profile.PremiumActivatedAt,
            SubscriptionId: profile.SubscriptionId
        );
    }
    public async Task ValidatePremiumAsync(Guid userId, CancellationToken ct)
    {
        var profile = await _profiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (profile is null)
            throw new UnauthorizedAccessException("Perfil profissional não encontrado.");

        if (string.IsNullOrWhiteSpace(profile.SubscriptionId))
            throw new UnauthorizedAccessException("Você ainda não possui uma assinatura ativa.");

        if (profile.SubscriptionStatus is null)
            throw new UnauthorizedAccessException("Sua assinatura está inativa.");

        switch (profile.SubscriptionStatus)
        {
            case SubscriptionStatus.Active:
                break;

            case SubscriptionStatus.Cancelled:
                throw new UnauthorizedAccessException("Sua assinatura foi cancelada.");

            case SubscriptionStatus.Paused:
                throw new UnauthorizedAccessException("Sua assinatura está pausada.");

            case SubscriptionStatus.ChargeFailed:
                throw new UnauthorizedAccessException("Seu pagamento falhou. Atualize sua forma de pagamento.");
        }

        if (profile.NextBillingDate.HasValue &&
            profile.NextBillingDate.Value < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Sua assinatura expirou. Atualize o pagamento.");
        }
    }

}
