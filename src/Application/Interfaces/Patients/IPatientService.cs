using Application.DTO.Patients;

namespace Application.Interfaces.Patients;

public interface IPatientService
{
    Task<Guid> CreateAsync(CreatePatientRequest request, Guid userAccountId, CancellationToken ct);
    Task<PatientDetailDto> GetByIdAsync(Guid patientId, Guid requesterId, string requesterRole, CancellationToken ct);
    Task<PatientDetailDto> GetMyProfileAsync(Guid userAccountId, CancellationToken ct);
    Task<IEnumerable<PatientListItemDto>> GetByProfessionalAsync(Guid professionalId, CancellationToken ct);
    Task UpdateAsync(UpdatePatientRequest request, Guid requesterId, string requesterRole, CancellationToken ct);
    Task UnassignProfessionalAsync(Guid patientId, Guid professionalId, CancellationToken ct);
}
