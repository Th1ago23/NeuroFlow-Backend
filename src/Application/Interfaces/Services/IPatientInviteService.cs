using Application.DTO.Patients;

namespace Application.Interfaces.Services;

public interface IPatientInviteService
{
    Task<PatientInviteDto> CreateInviteAsync (Guid professionalUserId, CancellationToken ct = default);
    Task<PatientInviteDto> ValidateInviteAsync (Guid token, CancellationToken ct = default);

}
