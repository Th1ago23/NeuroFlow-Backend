namespace Application.DTO.Thoughts;

public sealed record ThoughtCategoryDto(
    int Id,
    string Name,
    string IconKey
);
