using Application.DTO.Mood;

namespace Application.Interfaces.Mood;

public interface IMoodEntryService
{
    Task<Guid> CreateAsync(CreateMoodEntryRequest request);
    Task<IEnumerable<MoodEntryDto>> GetByPatientAsync(Guid patientId);
    Task<MoodStatsDto> GetStatsAsync(Guid patientId);
}
