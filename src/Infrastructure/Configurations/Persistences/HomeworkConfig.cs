using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences;

public class HomeworkConfig : IEntityTypeConfiguration<Homework>
{
    public void Configure(EntityTypeBuilder<Homework> builder)
    {
        builder.HasKey(h => h.Id);

        builder.HasOne(h => h.Patient)
            .WithMany(p => p.Homework)
            .HasForeignKey(h => h.PatientId);

        builder.HasOne(h => h.Professional)
            .WithMany()
            .HasForeignKey(h => h.ProfessionalUserId);

        builder.Property(h => h.Title)
            .IsRequired()
            .HasMaxLength(200);
    }
}
