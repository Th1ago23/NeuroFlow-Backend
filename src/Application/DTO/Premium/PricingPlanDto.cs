using Domain.Enums;

namespace Application.DTO.Premium;

public record PricingPlanDto(PremiumTier Tier,string Name,decimal Price,string Description);
