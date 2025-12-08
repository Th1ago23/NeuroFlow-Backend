namespace Infrastructure.Security;

public class MercadoPagoSettings
{
    public string AccessToken { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public string SuccessUrl { get; set; } = "https://seuapp.com/success";
}
