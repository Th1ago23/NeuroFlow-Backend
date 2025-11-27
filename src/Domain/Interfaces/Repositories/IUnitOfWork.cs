namespace Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IPatientRepository Patients { get; }
    IProfessionalProfileRepository ProfessionalProfiles { get; }
    IAppointmentRepository Appointments { get; }
    IClinicalAssessmentRepository ClinicalAssessments { get; }
    IClinicalNoteRepository ClinicalNotes { get; }
    IMoodEntryRepository MoodEntries { get; }
    IThoughtEntryRepository ThoughtEntries { get; }
    IPatientInviteRepository PatientInvites { get; }


    Task<int> CommitAsync();
    Task RollbackAsync();
}
