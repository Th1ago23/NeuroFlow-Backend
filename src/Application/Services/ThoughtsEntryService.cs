using Application.DTO.Thoughts;
using Application.Interfaces.Thoughts;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class ThoughtEntryService : IThoughtEntryService
{
    private readonly IThoughtEntryRepository _thoughtRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IUnitOfWork _uow;

    public ThoughtEntryService(
        IThoughtEntryRepository thoughtRepo,
        IPatientRepository patientRepo,
        IUnitOfWork uow)
    {
        _thoughtRepo = thoughtRepo;
        _patientRepo = patientRepo;
        _uow = uow;
    }

    public async Task<Guid> CreateAsync(CreateThoughtEntryRequest request)
    {
        var patient = await _patientRepo.GetByIdAsync(request.PatientId);
        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        var entry = new ThoughtEntry(
            request.PatientId,
            request.CategoryId,
            request.Notes,
            request.IsVisibleToProfessional
        );

        await _thoughtRepo.AddAsync(entry);
        await _uow.CommitAsync();

        return entry.Id;
    }

    public async Task<IEnumerable<ThoughtEntryDto>> GetByPatientAsync(Guid patientId)
    {
        var list = await _thoughtRepo.GetByPatientAsync(patientId);

        return list.Select(t => new ThoughtEntryDto(
            t.Id,
            t.CategoryId,
            t.Category.Name,
            t.Category.IconKey,
            t.Notes,
            t.IsVisibleToProfessional,
            t.CreatedAt
        ));
    }
}
