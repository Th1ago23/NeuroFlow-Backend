using Application.DTO.Thoughts;
using Domain.Entities;

namespace Application.Mappings.Thoughts;

public static class ThoughtMappings
{
    public static ThoughtCategoryDto ToDto(this ThoughtCategory category)
        => new(
            Id: category.Id,
            Name: category.Name,
            IconKey: category.IconKey
        );
    public static ThoughtEntryDto ToDto(this ThoughtEntry entry)
        => new(
            Id: entry.Id,
            CategoryId: entry.CategoryId,
            CategoryName: entry.Category.Name,
            IconKey: entry.Category.IconKey,
            Notes: entry.Notes,
            IsVisibleToProfessional: entry.IsVisibleToProfessional,
            CreatedAt: entry.CreatedAt
        );

    public static ThoughtEntry ToEntity(this CreateThoughtEntryRequest request)
    {
        return new ThoughtEntryBuilder()
            .WithId(Guid.NewGuid())
            .WithPatientId(request.PatientId)
            .WithCategoryId(request.CategoryId)
            .WithNotes(request.Notes)
            .WithVisibility(request.IsVisibleToProfessional)
            .WithCreatedAt(DateTime.UtcNow)
            .Build();
    }
}
