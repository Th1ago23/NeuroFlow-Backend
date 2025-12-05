using Application.DTO.Mood;
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

    public MoodEntryService(IMoodEntryRepository moodRepo,IPatientRepository patientRepo,IUnitOfWork uow)
    {
        _moodRepo = moodRepo;
        _patientRepo = patientRepo;
        _uow = uow;
    }

    public async Task<MoodEntryDto> CreateAsync(CreateMoodEntryRequest request,Guid patientUserId,CancellationToken ct)
    {
        var patient = await _patientRepo.GetByUserId(patientUserId, ct);

        if (patient is null)
            throw new UnauthorizedAccessException("Somente pacientes podem registrar humor.");

        if (!Enum.IsDefined(typeof(MoodLevel), request.Level))
            throw new ArgumentException("Nível de humor inválido.");

        var entry = new MoodEntry(
            patientId: patient.Id,
            level: request.Level,
            note: request.Note
        );

        await _moodRepo.AddAsync(entry, ct);
        await _uow.CommitAsync();

        return entry.ToDto();
    }

    public async Task<MoodEntryDto> UpdateAsync(Guid id,UpdateMoodEntryRequest request,Guid patientUserId,CancellationToken ct)
    {
        var entry = await _moodRepo.GetByIdAsync(id, ct);

        if (entry is null)
            throw new KeyNotFoundException("Registro não encontrado.");

        var patient = await _patientRepo.GetByIdAsync(entry.PatientId, ct);

        if (patient is null || patient.UserAccountId != patientUserId)
            throw new UnauthorizedAccessException("Você não pode editar esse registro.");

        if (!Enum.IsDefined(typeof(MoodLevel), request.Level))
            throw new ArgumentException("Nível de humor inválido.");

        entry.Update(request.Level, request.Note);

        _moodRepo.Update(entry);
        await _uow.CommitAsync();

        return entry.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id,Guid patientUserId,CancellationToken ct)
    {
        var entry = await _moodRepo.GetByIdAsync(id, ct);

        if (entry is null)
            return false;

        var patient = await _patientRepo.GetByIdAsync(entry.PatientId, ct);

        if (patient is null || patient.UserAccountId != patientUserId)
            throw new UnauthorizedAccessException("Você não pode excluir esse registro.");

        _moodRepo.Remove(entry);
        await _uow.CommitAsync();

        return true;
    }

    public async Task<IEnumerable<MoodEntryDto>> GetRecentAsync(Guid patientId,int limit,Guid requesterId,string requesterRole,CancellationToken ct)
    {
        var patient = await _patientRepo.GetByIdAsync(patientId, ct);

        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        ValidateAccess(patient, requesterId, requesterRole);

        var entries = await _moodRepo.GetRecentAsync(patientId, limit, ct);
        return entries.Select(e => e.ToDto());
    }

    public async Task<MoodStatsDto> GetStatsAsync(
        Guid patientId,
        Guid requesterId,
        string requesterRole,
        CancellationToken ct)
    {
        var patient = await _patientRepo.GetByIdAsync(patientId, ct);

        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        ValidateAccess(patient, requesterId, requesterRole);

        var now = DateTime.UtcNow;
        var monthStart = now.AddDays(-30);
        var weekStart = now.AddDays(-7);

        var moods = await _moodRepo.GetByDateRangeAsync(patientId, monthStart, now, ct);

        if (!moods.Any())
            return new MoodStatsDto(0, 0, 0);

        var weekly = moods.Where(m => m.CreatedAt >= weekStart);

        double weeklyAvg = weekly.Any() ? weekly.Average(m => (int)m.Level) : 0;
        double monthlyAvg = moods.Average(m => (int)m.Level);

        return new MoodStatsDto(
            Count: moods.Count(),
            WeeklyAverage: Math.Round(weeklyAvg, 2),
            MonthlyAverage: Math.Round(monthlyAvg, 2)
        );
    }

    private static void ValidateAccess(Patient patient, Guid requesterId, string role)
    {
        switch (role)
        {
            case "Patient":
                if (patient.UserAccountId != requesterId)
                    throw new UnauthorizedAccessException("Acesso negado.");
                break;

            case "Professional":
                if (patient.OwnerProfessionalId != requesterId)
                    throw new UnauthorizedAccessException("Acesso negado.");
                break;

            default:
                throw new UnauthorizedAccessException("Tipo de usuário não autorizado.");
        }
    }

    public async Task<IEnumerable<MoodEntryDto>> GetByDateRangeAsync(Guid patientId, DateTime start, DateTime end, Guid requesterId, string requesterRole, CancellationToken ct)
    {
        var patient = await _patientRepo.GetByIdAsync(patientId, ct);

        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        if (requesterRole == "Patient")
        {
            if (patient.UserAccountId != requesterId)
                throw new UnauthorizedAccessException("Você não pode acessar esses dados.");
        }
        else if (requesterRole == "Professional")
        {
            if (patient.OwnerProfessionalId != requesterId)
                throw new UnauthorizedAccessException("Você não pode acessar dados deste paciente.");
        }
        else
        {
            throw new UnauthorizedAccessException("Tipo de usuário não autorizado.");
        }

        var entries = await _moodRepo.GetByDateRangeAsync(patientId, start, end, ct);

        return entries.Select(e => e.ToDto());
    }
}
