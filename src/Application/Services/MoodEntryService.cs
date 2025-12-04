using Application.DTO.Mood;
using Application.Interfaces.Mood;
using Application.Interfaces.Services;
using Application.Mappings.Mood;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class MoodEntryService : IMoodEntryService
{
    private readonly IMoodEntryRepository _moodRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IUnitOfWork _uow;

    public MoodEntryService(
        IMoodEntryRepository moodRepo,
        IPatientRepository patientRepo,
        IUnitOfWork uow)
    {
        _moodRepo = moodRepo;
        _patientRepo = patientRepo;
        _uow = uow;
    }

    // ------------------ CREATE ------------------
    public async Task<MoodEntryDto> CreateAsync(CreateMoodEntryRequest request, Guid professionalId, CancellationToken ct)
    {
        var patient = await _patientRepo.GetByIdAsync(request.PatientId, ct);

        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        if (patient.OwnerProfessionalId != professionalId)
            throw new UnauthorizedAccessException("Você não tem permissão para cadastrar humor deste paciente.");

        if (!Enum.IsDefined(typeof(MoodLevel), request.Level))
            throw new ArgumentException("Nível de humor inválido.");


        var entry = new MoodEntry(patient.Id, request.Level, request.Note);

        await _moodRepo.AddAsync(entry, ct);
        await _uow.CommitAsync();

        return entry.ToDto();
    }

    // ------------------ GET RECENT ------------------
    public async Task<IEnumerable<MoodEntryDto>> GetRecentAsync(Guid patientId, int limit, Guid professionalId, CancellationToken ct)
    {
        var patient = await _patientRepo.GetByIdAsync(patientId, ct);

        if (patient is null || patient.OwnerProfessionalId != professionalId)
            throw new UnauthorizedAccessException("Acesso negado ao paciente.");

        var entries = await _moodRepo.GetRecentAsync(patientId, limit, ct);

        return entries.Select(e => e.ToDto());
    }

    // ------------------ GET BY DATE RANGE ------------------
    public async Task<IEnumerable<MoodEntryDto>> GetByDateRangeAsync(Guid patientId, DateTime start, DateTime end, Guid professionalId, CancellationToken ct)
    {
        var patient = await _patientRepo.GetByIdAsync(patientId, ct);

        if (patient is null || patient.OwnerProfessionalId != professionalId)
            throw new UnauthorizedAccessException("Acesso negado ao paciente.");

        var entries = await _moodRepo.GetByDateRangeAsync(patientId, start, end, ct);

        return entries.Select(e => e.ToDto());
    }

    // ------------------ UPDATE ------------------
    public async Task<MoodEntryDto> UpdateAsync(UpdateMoodEntryRequest request, Guid professionalId, CancellationToken ct)
    {
        var entry = await _moodRepo.GetByIdAsync(request.Id, ct);

        if (entry is null)
            throw new KeyNotFoundException("Registro de humor não encontrado.");

        var patient = await _patientRepo.GetByIdAsync(entry.PatientId, ct);

        if (patient.OwnerProfessionalId != professionalId)
            throw new UnauthorizedAccessException("Você não pode editar humor deste paciente.");

        entry.Update((MoodLevel)request.Level, request.Note);

        _moodRepo.Update(entry);
        await _uow.CommitAsync();

        return entry.ToDto();
    }

    // ------------------ DELETE ------------------
    public async Task<bool> DeleteAsync(Guid id, Guid professionalId, CancellationToken ct)
    {
        var entry = await _moodRepo.GetByIdAsync(id, ct);

        if (entry is null)
            return false;

        var patient = await _patientRepo.GetByIdAsync(entry.PatientId, ct);

        if (patient.OwnerProfessionalId != professionalId)
            throw new UnauthorizedAccessException("Você não pode excluir humor deste paciente.");

        _moodRepo.Remove(entry);
        await _uow.CommitAsync();

        return true;
    }
}
