using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persistences;

public class ThoughtCategoryConfig : IEntityTypeConfiguration<ThoughtCategory>
{
    public void Configure(EntityTypeBuilder<ThoughtCategory> builder)
    {
        builder.ToTable("ThoughtCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
               .HasMaxLength(80)
               .IsRequired();

        builder.Property(x => x.IconKey)
               .HasMaxLength(50)
               .IsRequired();
    }
}
