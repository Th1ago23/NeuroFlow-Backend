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

    public async Task<CreateSubscriptionResponse> CreateSubscriptionAsync(Guid professionalUserId,PremiumTier tier,CancellationToken ct)
    {
        var profile = await _profiles.GetByUserIdAsync(professionalUserId, ct);

        if (profile is null)
            throw new InvalidOperationException("Perfil profissional não encontrado.");

        var planId = tier switch
        {
            PremiumTier.Premium => _settings.PlanPremiumId,
            PremiumTier.PremiumPlus => _settings.PlanPremiumPlusId,
            _ => throw new InvalidOperationException("Plano inválido.")
        };

        var requestBody = new
        {
            preapproval_plan_id = planId,
            payer_email = profile.User.Email.Address,
            back_url = _settings.SuccessUrl
        };

        var response = await _http.PostAsJsonAsync(
            "https://api.mercadopago.com/preapproval", requestBody, ct);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<dynamic>(ct);

        string initPoint = json.init_point;
        string subscriptionId = json.id;

        profile.MarkSubscriptionCreated(subscriptionId, tier);

        await _uow.CommitAsync();

        return new CreateSubscriptionResponse(initPoint, subscriptionId);
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
                profile.ResetPaymentFailures();
                break;

            case "pending":
                profile.MarkSubscriptionPending();
                break;

            case "paused":
                profile.MarkPaymentFailedWithRetry();
                break;

            case "cancelled":
            case "cancelled_by_user":
            case "cancelled_by_merchant":
                profile.CancelPremium();
                break;

            case "rejected":
            case "charged_back":
            case "charged_back_by_bank":
                profile.MarkPaymentFailedWithRetry();
                break;

            default:
                profile.MarkPaymentFailedWithRetry();
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
            SubscriptionId: profile.SubscriptionId,
            Tier: profile.Tier
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
    public async Task CancelSubscriptionAsync(Guid userId, CancellationToken ct)
    {
        var profile = await _profiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (profile is null)
            throw new InvalidOperationException("Perfil profissional não encontrado.");

        if (string.IsNullOrWhiteSpace(profile.SubscriptionId))
            throw new InvalidOperationException("Nenhuma assinatura ativa foi encontrada.");

        var requestBody = new
        {
            status = "cancelled"
        };

        var response = await _http.PutAsJsonAsync(
            $"https://api.mercadopago.com/preapproval/{profile.SubscriptionId}",
            requestBody,
            ct);

        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Erro ao cancelar assinatura: {msg}");
        }

        profile.CancelPremium();

        await _uow.CommitAsync();
    }

}
