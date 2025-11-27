using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NeuroFlowContext _context;

        public IUserRepository Users { get; }
        public IPatientRepository Patients { get; }
        public IProfessionalProfileRepository ProfessionalProfiles { get; }
        public IAppointmentRepository Appointments { get; }
        public IClinicalAssessmentRepository ClinicalAssessments { get; }
        public IClinicalNoteRepository ClinicalNotes { get; }
        public IMoodEntryRepository MoodEntries { get; }
        public IThoughtEntryRepository ThoughtEntries { get; }
        public IPatientInviteRepository PatientInvites { get; }


        public UnitOfWork(
            NeuroFlowContext context,
            IPatientInviteRepository patientInvites,
            IUserRepository users,
            IPatientRepository patients,
            IProfessionalProfileRepository professionalProfiles,
            IAppointmentRepository appointments,
            IClinicalAssessmentRepository clinicalAssessments,
            IClinicalNoteRepository clinicalNotes,
            IMoodEntryRepository moodEntries,
            IThoughtEntryRepository thoughtEntries)
        {
            _context = context;
            PatientInvites = patientInvites;
            Users = users;
            Patients = patients;
            ProfessionalProfiles = professionalProfiles;
            Appointments = appointments;
            ClinicalAssessments = clinicalAssessments;
            ClinicalNotes = clinicalNotes;
            MoodEntries = moodEntries;
            ThoughtEntries = thoughtEntries;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public Task RollbackAsync()
        {
            return Task.CompletedTask;
        }
    }
}
