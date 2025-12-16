using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Application.Interfaces.Premium;
using System.Security.Claims;
using Domain.Enums;

namespace API.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PremiumOnlyAttribute : Attribute, IAsyncAuthorizationFilter
{
    public PremiumTier RequiredTier { get; }

    public PremiumOnlyAttribute(PremiumTier requiredTier = PremiumTier.Premium)
    {
        RequiredTier = requiredTier;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;

        var userIdString = httpContext.User.FindFirstValue("uid");
        if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            context.Result = new UnauthorizedObjectResult("Usuário não autenticado.");
            return;
        }

        var role = httpContext.User.FindFirstValue(ClaimTypes.Role);
        if (role is null || !role.Equals("Professional", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedObjectResult("Apenas profissionais podem acessar este recurso.");
            return;
        }

        var premiumService = httpContext.RequestServices.GetRequiredService<IPremiumService>();

        var status = await premiumService.GetStatusAsync(userId, httpContext.RequestAborted);

        if (status.Tier < RequiredTier)
        {
            context.Result = new UnauthorizedObjectResult(
                $"Plano insuficiente. Requer: {RequiredTier}, atual: {status.Tier}.");
            return;
        }
    }
}

