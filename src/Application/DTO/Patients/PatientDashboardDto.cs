using Application.DTO.Clinical;
using Application.DTO.Mood;
using Application.DTO.Thoughts;

namespace Application.DTO.Patients
{
    public sealed record PatientDashboardDto(
        PatientDetailDto Patient,
        IEnumerable<MoodEntryDto> RecentMoods,
        IEnumerable<ThoughtEntryDto> RecentThoughts,
        IEnumerable<ClinicalNoteDto> RecentNotes
    );

}
