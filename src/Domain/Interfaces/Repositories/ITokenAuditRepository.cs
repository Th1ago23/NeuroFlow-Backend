using Domain.Entities.Security;

namespace Domain.Interfaces.Repositories;

public interface ITokenAuditRepository
{
    Task AddAsync(TokenAudit audit, CancellationToken ct = default);
    Task<TokenAudit?> GetByJtiAsync(string jti, CancellationToken ct = default);
    Task<bool> ExistsActiveTokenAsync(Guid userId, string jti, CancellationToken ct = default);
    Task RevokeAsync(string jti, CancellationToken ct = default);
}
