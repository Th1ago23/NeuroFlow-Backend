using Application.DTO.Common;

namespace Application.DTO.Patients;

public sealed record PatientSummaryDto(
    Guid Id,
    NameDto Name,
    DateOnly BirthDate,
    GenderDto Gender,
    bool IsActive
);
