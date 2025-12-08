namespace Application.DTO.Premium;

public sealed record MercadoPagoWebhookData(
    string Action,
    string Type,
    MercadoPagoWebhookResource Data
);
