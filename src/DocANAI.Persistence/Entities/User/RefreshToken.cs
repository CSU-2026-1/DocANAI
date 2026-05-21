using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Entities.User;

public sealed class RefreshToken : AuditableEntity<IdOf<RefreshToken>>
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public string? CreatedByIp { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }

    public IdOf<User> UserId { get; private set; }
    public User? User { get; private set; }

    private RefreshToken() {  }

    private RefreshToken(
        IdOf<RefreshToken> id,
        string token,
        DateTime expiresAt,
        string createdByIp,
        IdOf<User> userId
    ) : base(id)
    {
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
        CreatedByIp = createdByIp;
        UserId = userId;
    }

    public static RefreshToken Create(
        string token,
        DateTime expiresAt,
        string createdByIp,
        IdOf<User> userId)
        => new(IdOf<RefreshToken>.New(), token, expiresAt, createdByIp, userId);

    public void Revoke(string revokeByIp)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokeByIp;
    }
}