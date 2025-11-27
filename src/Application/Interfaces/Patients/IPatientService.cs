using Application.DTO.Patients;

namespace Application.Interfaces.Patients
{
    public interface IPatientService
    {
        Task<Guid> CreateAsync(CreatePatientRequest request);
        Task UpdateAsync(UpdatePatientRequest request);
        Task<PatientDetailDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<PatientSummaryDto>> GetByProfessionalAsync(Guid professionalUserId);
        Task DeactivateAsync(Guid patientId);
        Task<PatientDashboardDto> GetDashboardAsync(Guid patientId);
    }
}
