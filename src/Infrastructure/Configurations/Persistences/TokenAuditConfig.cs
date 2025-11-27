using Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences;

public class TokenAuditConfig : IEntityTypeConfiguration<TokenAudit>
{
    public void Configure(EntityTypeBuilder<TokenAudit> builder)
    {
        builder.ToTable("TokenAudits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Jti).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.IssuedAt).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();

        builder.Property(x => x.Revoked)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.Jti).IsUnique();
        builder.HasIndex(x => x.UserId);
    }
}
