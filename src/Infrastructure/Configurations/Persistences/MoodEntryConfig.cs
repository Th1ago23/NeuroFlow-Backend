using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences;

public class MoodEntryConfig : IEntityTypeConfiguration<MoodEntry>
{
    public void Configure(EntityTypeBuilder<MoodEntry> builder)
    {
        builder.ToTable("MoodEntries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.Note)
               .HasMaxLength(300);

        builder.Property(x => x.Level)
               .HasConversion<int>()
               .IsRequired();

        builder.HasOne(x => x.Patient)
               .WithMany(p => p.MoodEntries)
               .HasForeignKey(x => x.PatientId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
