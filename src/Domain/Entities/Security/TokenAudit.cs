namespace Domain.Entities.Security;

public class TokenAudit
{
    private TokenAudit() { }

    public TokenAudit(Guid userId, string jti, DateTime issuedAt, DateTime expiresAt, string? ip, string? userAgent)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Jti = jti;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
        IpAddress = ip;
        UserAgent = userAgent;
        Revoked = false;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }

    public string Jti { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    public bool Revoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public void Revoke()
    {
        if (!Revoked)
        {
            Revoked = true;
            RevokedAt = DateTime.UtcNow;
        }
    }
}
