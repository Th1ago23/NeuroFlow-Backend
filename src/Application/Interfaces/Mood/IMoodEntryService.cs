using Application.DTO.Mood;

namespace Application.Interfaces.Mood;

public interface IMoodEntryService
{
    Task<MoodEntryDto> CreateAsync(CreateMoodEntryRequest request, Guid professionalId, CancellationToken ct);
    Task<IEnumerable<MoodEntryDto>> GetRecentAsync(Guid patientId, int limit, Guid professionalId, CancellationToken ct);
    Task<IEnumerable<MoodEntryDto>> GetByDateRangeAsync(Guid patientId, DateTime start, DateTime end, Guid professionalId, CancellationToken ct);
    Task<MoodEntryDto> UpdateAsync(UpdateMoodEntryRequest request, Guid professionalId, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, Guid professionalId, CancellationToken ct);
}
