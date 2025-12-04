namespace Application.Interfaces.Notification;

internal interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string htmlMessage, CancellationToken ct = default);
}
