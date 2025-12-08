using System.Text.Json;
using Application.DTO.Premium;

namespace Infrastructure.Payments.MercadoPago;

public static class PremiumWebhookParser
{
    public static MercadoPagoWebhookData Parse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        string action = root.GetProperty("action").GetString() ?? "unknown";
        string type = root.GetProperty("type").GetString() ?? "unknown";

        string id = root.TryGetProperty("data", out var dataProp) &&
                    dataProp.TryGetProperty("id", out var idProp)
            ? idProp.GetString() ?? ""
            : root.TryGetProperty("id", out var idFallback)
                ? idFallback.GetRawText().Replace("\"", "")
                : "";

        return new MercadoPagoWebhookData(
            Action: action,
            Type: type,
            Data: new MercadoPagoWebhookResource(id)
        );
    }
}
