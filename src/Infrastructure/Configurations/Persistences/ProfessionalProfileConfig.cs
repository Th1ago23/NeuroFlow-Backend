using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences;

public class ProfessionalProfileConfig : IEntityTypeConfiguration<ProfessionalProfile>
{
    public void Configure(EntityTypeBuilder<ProfessionalProfile> builder)
    {
        builder.ToTable("ProfessionalProfiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentNumber)
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.Speciality)
               .HasMaxLength(100)
               .IsRequired();

        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(120);
            address.Property(a => a.Number).HasColumnName("Number").HasMaxLength(10);
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(80);
            address.Property(a => a.State).HasColumnName("State").HasMaxLength(40);
            address.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(15);
        });

        builder.HasOne(p => p.User)
               .WithOne(u => u.ProfessionalProfile)
               .HasForeignKey<ProfessionalProfile>(p => p.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
