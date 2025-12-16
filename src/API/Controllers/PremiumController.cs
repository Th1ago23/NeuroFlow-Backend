using Application.Common.Responses;
using Application.DTO.Premium;
using Application.Interfaces.Premium;
using API.Filters; // PremiumOnly
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Infrastructure.Payments.MercadoPago;
using Domain.Enums;

namespace API.Controllers
{
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
        public async Task<IActionResult> CreateSubscription(
            [FromQuery] PremiumTier tier,
            CancellationToken ct)
        {
            var userId = GetUserId();
            var result = await _premiumService.CreateSubscriptionAsync(userId, tier, ct);
            return Ok(ApiResponse<CreateSubscriptionResponse>.Ok(result));
        }

        [HttpGet("status")]
        [Authorize(Roles = "Professional")]
        public async Task<IActionResult> GetStatus(CancellationToken ct)
        {
            var userId = GetUserId();
            var result = await _premiumService.GetStatusAsync(userId, ct);

            return Ok(ApiResponse<PremiumStatusDto>.Ok(result));
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook([FromBody] object raw, CancellationToken ct)
        {
            var json = raw.ToString();

            if (string.IsNullOrWhiteSpace(json))
                return BadRequest(ApiResponse.Fail("Payload inválido."));

            var parsed = PremiumWebhookParser.Parse(json);

            await _premiumService.ProcessWebhookAsync(parsed, ct);

            return Ok();
        }

        [HttpGet("example-protected")]
        [Authorize(Roles = "Professional")]
        [PremiumOnly]
        public IActionResult ExamplePremiumOnly()
        {
            return Ok(ApiResponse.Ok("Você tem acesso Premium 🚀"));
        }
        [HttpPost("cancel")]
        [Authorize(Roles = "Professional")]
        public async Task<IActionResult> Cancel(CancellationToken ct)
        {
            var userId = GetUserId();

            await _premiumService.CancelSubscriptionAsync(userId, ct);

            return Ok(ApiResponse.Ok("Assinatura cancelada com sucesso."));
        }
        [HttpGet("pricing")]
        [AllowAnonymous]
        public IActionResult GetPricing()
        {
            var plans = new[]
            {
        new PricingPlanDto(
            Tier: PremiumTier.Premium,
            Name: "Premium",
            Price: 29.99m,
            Description: "Acesso completo às funcionalidades profissionais."
        ),
        new PricingPlanDto(
            Tier: PremiumTier.PremiumPlus,
            Name: "Premium Plus",
            Price: 79.99m,
            Description: "Inclui IA avançada, transcrição e recomendações automáticas."
        )
    };

            return Ok(ApiResponse<IEnumerable<PricingPlanDto>>.Ok(plans));
        }
    }
}
