using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences;

public class AppointmentConfig : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ScheduledDateTime)
                .IsRequired();

        builder.Property(x => x.CreatedAt)
                .IsRequired();

        builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

        builder.HasOne(x => x.Professional)
                .WithMany()
                .HasForeignKey(x => x.ProfessionalUserId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Patient)
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
    }
}

