using Domain.Entities;

namespace Application.Mappings.Thoughts;

internal class ThoughtEntryBuilder
{
    private Guid _id;
    private Guid _patientId;
    private int _categoryId;
    private string? _notes;
    private bool _isVisibleToProfessional;
    private DateTime _createdAt;

    public ThoughtEntryBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ThoughtEntryBuilder WithPatientId(Guid patientId)
    {
        _patientId = patientId;
        return this;
    }

    public ThoughtEntryBuilder WithCategoryId(int categoryId)
    {
        _categoryId = categoryId;
        return this;
    }

    public ThoughtEntryBuilder WithNotes(string? notes)
    {
        _notes = notes;
        return this;
    }

    public ThoughtEntryBuilder WithVisibility(bool visible)
    {
        _isVisibleToProfessional = visible;
        return this;
    }

    public ThoughtEntryBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public ThoughtEntry Build()
    {
        var entry = (ThoughtEntry)Activator.CreateInstance(
            typeof(ThoughtEntry),
            nonPublic: true
        )!;

        typeof(ThoughtEntry).GetProperty("Id")!.SetValue(entry, _id);
        typeof(ThoughtEntry).GetProperty("PatientId")!.SetValue(entry, _patientId);
        typeof(ThoughtEntry).GetProperty("CategoryId")!.SetValue(entry, _categoryId);
        typeof(ThoughtEntry).GetProperty("Notes")!.SetValue(entry, _notes);
        typeof(ThoughtEntry).GetProperty("IsVisibleToProfessional")!.SetValue(entry, _isVisibleToProfessional);
        typeof(ThoughtEntry).GetProperty("CreatedAt")!.SetValue(entry, _createdAt);

        return entry;
    }
}
