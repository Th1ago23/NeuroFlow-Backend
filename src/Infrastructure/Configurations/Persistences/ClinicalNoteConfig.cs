using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences
{
    public class ClinicalNoteConfig : IEntityTypeConfiguration<ClinicalNote>
    {
        public void Configure(EntityTypeBuilder<ClinicalNote> builder)
        {
            builder.ToTable("ClinicalNotes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Note)
                   .HasMaxLength(2000)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.HasOne(x => x.Patient)
                   .WithMany(p => p.Notes)
                   .HasForeignKey(x => x.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
