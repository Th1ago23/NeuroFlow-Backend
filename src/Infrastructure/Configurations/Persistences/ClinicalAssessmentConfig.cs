using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences;

public class ClinicalAssessmentConfig : IEntityTypeConfiguration<ClinicalAssessment>
{
    public void Configure(EntityTypeBuilder<ClinicalAssessment> builder)
    {
        builder.ToTable("ClinicalAssessments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Date)
               .IsRequired();

        builder.Property(x => x.Summary)
               .HasMaxLength(2000)
               .IsRequired();

        builder.Property(x => x.Diagnosis)
               .HasMaxLength(200);

        builder.Property(x => x.Recommendations)
               .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.HasOne(x => x.Professional)
               .WithMany()
               .HasForeignKey(x => x.ProfessionalUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Patient)
               .WithMany(p => p.Assessments)
               .HasForeignKey(x => x.PatientId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
