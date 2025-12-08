using System.Text.Json.Serialization;

namespace Infrastructure.Payments.MercadoPago;


internal sealed class MercadoPagoCreateSubscriptionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("init_point")]
    public string InitPoint { get; set; } = string.Empty;
}
