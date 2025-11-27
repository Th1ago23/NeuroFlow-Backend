using Domain.Entities.Security;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TokenAuditRepository : ITokenAuditRepository
{
    private readonly NeuroFlowContext _db;

    public TokenAuditRepository(NeuroFlowContext db)
    {
        _db = db;
    }

    public async Task AddAsync(TokenAudit audit, CancellationToken ct = default)
    {
        await _db.TokenAudits.AddAsync(audit, ct);
    }

    public async Task<TokenAudit?> GetByJtiAsync(string jti, CancellationToken ct = default)
    {
        return await _db.TokenAudits.FirstOrDefaultAsync(x => x.Jti == jti, ct);
    }

    public async Task<bool> ExistsActiveTokenAsync(Guid userId, string jti, CancellationToken ct = default)
    {
        return await _db.TokenAudits
            .AnyAsync(x => x.UserId == userId && x.Jti == jti && !x.Revoked, ct);
    }

    public async Task RevokeAsync(string jti, CancellationToken ct = default)
    {
        var audit = await GetByJtiAsync(jti, ct);
        if (audit != null)
        {
            audit.Revoke();
        }
    }
}
