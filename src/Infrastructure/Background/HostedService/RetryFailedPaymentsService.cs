using Domain.Interfaces.Repositories;
using Infrastructure.Payments.MercadoPago;
using Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Infrastructure.Background.HostedService;

public class RetryFailedPaymentsService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RetryFailedPaymentsService> _logger;

    public RetryFailedPaymentsService(
        IServiceScopeFactory scopeFactory,
        ILogger<RetryFailedPaymentsService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessRetries(stoppingToken);

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcessRetries(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();

        var profiles = scope.ServiceProvider.GetRequiredService<IProfessionalProfileRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var http = scope.ServiceProvider.GetRequiredService<HttpClient>();
        var settings = scope.ServiceProvider.GetRequiredService<IOptions<MercadoPagoSettings>>().Value;

        var now = DateTime.UtcNow;

        var list = await profiles.GetProfilesWithRetryDueAsync(now, ct);

        foreach (var profile in list)
        {
            try
            {
                var resp = await http.GetFromJsonAsync<MercadoPagoPreapprovalResponse>(
                    $"https://api.mercadopago.com/preapproval/{profile.SubscriptionId}", ct);

                if (resp is null)
                    continue;

                if (resp.Status == "authorized")
                {
                    profile.ResetPaymentFailures();
                    profile.UpdateBilling(resp.NextPaymentDate ?? now);
                }
                else
                {
                    profile.MarkPaymentFailedWithRetry();
                }

                await uow.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar retentativa da assinatura {ProfileId}", profile.Id);
            }
        }
    }
}
