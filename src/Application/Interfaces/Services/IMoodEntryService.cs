using Application.DTO.Mood;

namespace Application.Interfaces.Services;

public interface IMoodEntryService
{
    Task<MoodEntryDto> CreateAsync(CreateMoodEntryRequest request,Guid patientUserId,CancellationToken ct);
    Task<MoodEntryDto> UpdateAsync(Guid id,UpdateMoodEntryRequest request,Guid patientUserId,CancellationToken ct);
    Task<bool> DeleteAsync(Guid id,Guid patientUserId,CancellationToken ct);
    Task<IEnumerable<MoodEntryDto>> GetRecentAsync(Guid patientId,int limit,Guid requesterId,string requesterRole,CancellationToken ct);
    Task<IEnumerable<MoodEntryDto>> GetByDateRangeAsync(Guid patientId,DateTime start,DateTime end,Guid requesterId,string requesterRole,CancellationToken ct);
    Task<MoodStatsDto> GetStatsAsync(Guid patientId,Guid requesterId,string requesterRole,CancellationToken ct);
}
