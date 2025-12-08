namespace Application.DTO.Premium;

public sealed record PremiumStatusDto(bool IsPremium,string Status,DateTime? NextBillingDate,DateTime? ActivatedAt,string? SubscriptionId);

