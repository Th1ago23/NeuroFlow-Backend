using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences;

public class HomeworkConfig : IEntityTypeConfiguration<Homework>
{
    public void Configure(EntityTypeBuilder<Homework> builder)
    {
        builder.ToTable("Homeworks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .HasMaxLength(120)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasMaxLength(500);

        builder.Property(x => x.ExpirationTime)
               .IsRequired();

        builder.Property(x => x.IsDone)
               .IsRequired();
    }
}
