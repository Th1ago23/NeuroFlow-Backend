using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences;

public class PatientConfig : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(p => p.Name, n =>
        {
            n.Property(p => p.FirstName).HasColumnName("FirstName").HasMaxLength(60).IsRequired();
            n.Property(p => p.LastName).HasColumnName("LastName").HasMaxLength(60).IsRequired();
        });

        builder.OwnsOne(p => p.Gender, gender =>
        {
            gender.Property(g => g.Type)
                  .HasColumnName("Gender")
                  .HasConversion<int>()
                  .IsRequired();

            gender.Property(g => g.CustomValue)
                  .HasColumnName("GenderCustomValue")
                  .HasMaxLength(50);
        });

        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(120);
            address.Property(a => a.Number).HasColumnName("Number").HasMaxLength(10);
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(80);
            address.Property(a => a.State).HasColumnName("State").HasMaxLength(40);
            address.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(15);
        });

        builder.Property(x => x.IsActive)
               .IsRequired();

        builder.HasOne(p => p.OwnerProfessional)
               .WithMany()
               .HasForeignKey(p => p.OwnerProfessionalId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.UserAccount)
               .WithMany()
               .HasForeignKey(p => p.UserAccountId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
