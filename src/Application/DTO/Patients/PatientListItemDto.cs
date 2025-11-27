using Application.DTO.Common;

namespace Application.DTO.Patients
{
    public sealed record PatientListItemDto(
        Guid Id,
        NameDto Name,
        bool IsActive
    );
}
