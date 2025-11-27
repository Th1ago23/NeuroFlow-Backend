using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences;

public class ThoughtEntryConfig : IEntityTypeConfiguration<ThoughtEntry>
{
    public void Configure(EntityTypeBuilder<ThoughtEntry> builder)
    {
        builder.ToTable("ThoughtEntries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.Notes)
               .HasMaxLength(500);

        builder.Property(x => x.IsVisibleToProfessional)
               .IsRequired();

        builder.HasOne(x => x.Patient)
               .WithMany(p => p.ThoughtEntries)
               .HasForeignKey(x => x.PatientId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Category)
               .WithMany()
               .HasForeignKey(x => x.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
