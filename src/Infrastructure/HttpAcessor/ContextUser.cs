using Domain.Interfaces.Http;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.HttpAcessor;

public class ContextUser : IContextUser
{
    private readonly IHttpContextAccessor _accessor;

    public ContextUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid UserId
    {
        get
        {
            var claim = _accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(claim, out Guid id) ? id : Guid.Empty;
        }
    }

    public string? Email
        => _accessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

    public string? Role
        => _accessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
}
