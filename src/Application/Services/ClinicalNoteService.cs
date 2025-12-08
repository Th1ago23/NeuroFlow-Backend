using Application.DTO.Clinical;
using Application.Interfaces.Clinical;
using Application.Interfaces.Logging;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.Services.Clinical
{
    public class ClinicalNoteService : IClinicalNoteService
    {
        private readonly IClinicalNoteRepository _noteRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IUnitOfWork _uow;
        private readonly IAppLogger<ClinicalNoteService> _logger;

        public ClinicalNoteService(
            IClinicalNoteRepository noteRepo,
            IPatientRepository patientRepo,
            IUnitOfWork uow,
            IAppLogger<ClinicalNoteService> logger)
        {
            _noteRepo = noteRepo;
            _patientRepo = patientRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Guid> CreateAsync(
            CreateClinicalNoteRequest request,
            Guid requesterId,
            string requesterRole)
        {
            _logger.LogInformation(
                "Iniciando criação de ClinicalNote para paciente {PatientId} por profissional {UserId}",
                request.PatientId, requesterId);

            ValidateProfessional(requesterRole);

            var patient = await _patientRepo.GetByIdAsync(request.PatientId)
                ?? throw new KeyNotFoundException("Paciente não encontrado.");

            ValidateOwnership(patient, requesterId);

            var note = new ClinicalNote(request.PatientId, request.Note);

            await _noteRepo.AddAsync(note);
            await _uow.CommitAsync();

            _logger.LogInformation("ClinicalNote criada com sucesso {NoteId}", note.Id);

            return note.Id;
        }

        public async Task<IEnumerable<ClinicalNoteDto>> GetByPatientAsync(
            Guid patientId,
            Guid requesterId,
            string requesterRole)
        {
            ValidateProfessional(requesterRole);

            var patient = await _patientRepo.GetByIdAsync(patientId)
                ?? throw new KeyNotFoundException("Paciente não encontrado.");

            ValidateOwnership(patient, requesterId);

            var notes = await _noteRepo.GetByPatientAsync(patientId);

            return notes.Select(n => new ClinicalNoteDto(
                n.Id,
                n.Note,
                n.CreatedAt
            ));
        }

        public async Task UpdateAsync(
            Guid noteId,
            string newText,
            Guid requesterId,
            string requesterRole)
        {
            ValidateProfessional(requesterRole);

            var note = await _noteRepo.GetByIdAsync(noteId)
                ?? throw new KeyNotFoundException("Nota não encontrada.");

            var patient = await _patientRepo.GetByIdAsync(note.PatientId)
                ?? throw new KeyNotFoundException("Paciente não encontrado.");

            ValidateOwnership(patient, requesterId);

            note.Update(newText);

            _noteRepo.Update(note);
            await _uow.CommitAsync();

            _logger.LogInformation("ClinicalNote atualizada {NoteId}", noteId);
        }

        public async Task DeleteAsync(
            Guid noteId,
            Guid requesterId,
            string requesterRole)
        {
            ValidateProfessional(requesterRole);

            var note = await _noteRepo.GetByIdAsync(noteId)
                ?? throw new KeyNotFoundException("Nota não encontrada.");

            var patient = await _patientRepo.GetByIdAsync(note.PatientId)
                ?? throw new KeyNotFoundException("Paciente não encontrado.");

            ValidateOwnership(patient, requesterId);

            _noteRepo.Remove(note);
            await _uow.CommitAsync();

            _logger.LogInformation("ClinicalNote removida {NoteId}", noteId);
        }

        private static void ValidateProfessional(string requesterRole)
        {
            if (requesterRole != "Professional")
                throw new UnauthorizedAccessException("Apenas profissionais podem acessar notas clínicas.");
        }

        private static void ValidateOwnership(Patient patient, Guid requesterId)
        {
            if (patient.OwnerProfessionalId != requesterId)
                throw new UnauthorizedAccessException(
                    "Você não tem permissão para acessar notas clínicas desse paciente.");
        }
    }
}
