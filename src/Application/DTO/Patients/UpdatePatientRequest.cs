using Application.DTO.Common;

namespace Application.DTO.Patients;

public sealed record UpdatePatientRequest(
    Guid Id,
    NameDto Name,
    DateOnly BirthDate,
    GenderDto Gender,
    AddressDto? Address,
    bool IsActive
);