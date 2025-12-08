using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Application.Interfaces.Premium;
using System.Security.Claims;

namespace Infrastructure.Security;

public class PremiumOnlyAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var premiumService = context.HttpContext.RequestServices.GetService(typeof(IPremiumService)) as IPremiumService;

        var userIdClaim = context.HttpContext.User.FindFirst("uid")?.Value;

        if (premiumService == null || userIdClaim == null)
        {
            context.Result = new UnauthorizedAccessException("Não autorizado.");
            return;
        }

        var userId = Guid.Parse(userIdClaim);

        try
        {
            await premiumService.ValidatePremiumAsync(userId, context.HttpContext.RequestAborted);
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Result = new UnauthorizedObjectResult(ex.Message);
        }
    }
}
