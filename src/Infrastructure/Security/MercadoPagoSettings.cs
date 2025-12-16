namespace Infrastructure.Security;

public class MercadoPagoSettings
{
    public string AccessToken { get; set; } = string.Empty;
    public string SuccessUrl { get; set; } = string.Empty;
    public string PlanPremiumId { get; set; } = string.Empty;
    public string PlanPremiumPlusId { get; set; } = string.Empty;
}
