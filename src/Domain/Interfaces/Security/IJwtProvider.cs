using Domain.Entities.Security;

namespace Domain.Interfaces.Security;

public interface IJwtProvider{
    Task<JwtTokenResult> GenerateTokenAsync(Guid userId,string email,string role,bool isVerifiedProfessional,bool isPremium,CancellationToken ct = default);
}

