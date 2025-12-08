using Application.DTO.Premium;

namespace Application.Interfaces.Premium;

public interface IPremiumService
{
    Task<CreateSubscriptionResponse> CreateSubscriptionAsync(Guid professionalUserId, CancellationToken ct);

    Task ProcessWebhookAsync(MercadoPagoWebhookData data, CancellationToken ct);
    Task<PremiumStatusDto> GetStatusAsync(Guid userId, CancellationToken ct);
    Task ValidatePremiumAsync(Guid userId, CancellationToken ct);
}
