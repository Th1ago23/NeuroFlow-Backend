using Application.DTO.Premium;
using Domain.Enums;

namespace Application.Interfaces.Premium;

public interface IPremiumService
{
    public Task<CreateSubscriptionResponse> CreateSubscriptionAsync(Guid userId, PremiumTier tier, CancellationToken ct);
    Task ProcessWebhookAsync(MercadoPagoWebhookData data, CancellationToken ct);
    Task<PremiumStatusDto> GetStatusAsync(Guid userId, CancellationToken ct);
    Task ValidatePremiumAsync(Guid userId, CancellationToken ct);
    Task CancelSubscriptionAsync(Guid userId, CancellationToken ct);
}
