using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities.Security;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _settings;
    private readonly ITokenAuditRepository _tokenAuditRepository;
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtProvider(
        IOptions<JwtSettings> settings,
        ITokenAuditRepository tokenAuditRepository,
        IUnitOfWork uow,
        IHttpContextAccessor httpContextAccessor)
    {
        _settings = settings.Value;
        _tokenAuditRepository = tokenAuditRepository;
        _uow = uow;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<JwtTokenResult> GenerateTokenAsync(
        Guid userId,
        string email,
        string role,
        bool isVerifiedProfessional,
        bool isPremium,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_settings.ExpirationMinutes);

        // jti = identificador único do token
        var jti = Guid.NewGuid().ToString();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, jti),
            new("uid", userId.ToString()),
            new("verified", isVerifiedProfessional.ToString().ToLowerInvariant()),
            new("premium", isPremium.ToString().ToLowerInvariant()),
            new(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Dados de auditoria
        var httpContext = _httpContextAccessor.HttpContext;
        var ip = httpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();

        var audit = new TokenAudit(
            userId: userId,
            jti: jti,
            issuedAt: now,
            expiresAt: expires,
            ip: ip,
            userAgent: userAgent
        );

        await _tokenAuditRepository.AddAsync(audit, ct);
        await _uow.CommitAsync();

        return new JwtTokenResult(
            AccessToken: tokenString,
            ExpiresAt: expires,
            Jti: jti
        );
    }
}
