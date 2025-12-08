using System.Text.Json.Serialization;

namespace Infrastructure.Payments.MercadoPago;

internal sealed class MercadoPagoPreapprovalResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("next_payment_date")]
    public DateTime? NextPaymentDate { get; set; }
}
