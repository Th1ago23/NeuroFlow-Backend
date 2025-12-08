using Application.Common.Responses;
using Application.DTO.Premium;
using Application.Interfaces.Premium;
using Infrastructure.Payments.MercadoPago;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/premium")]
public class PremiumController : ControllerBase
{
    private readonly IPremiumService _premiumService;

    public PremiumController(IPremiumService premiumService)
    {
        _premiumService = premiumService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue("uid")!);

    [HttpPost("subscribe")]
    [Authorize(Roles = "Professional")]
    public async Task<IActionResult> CreateSubscription(CancellationToken ct)
    {
        var userId = GetUserId();

        var result = await _premiumService.CreateSubscriptionAsync(userId, ct);

        return Ok(ApiResponse<CreateSubscriptionResponse>.Ok(result));
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook([FromBody] object raw, CancellationToken ct)
    {
        var json = raw.ToString();

        if (string.IsNullOrWhiteSpace(json))
            return BadRequest();

        var data = PremiumWebhookParser.Parse(json);

        await _premiumService.ProcessWebhookAsync(data, ct);
        return Ok();
    }
}
