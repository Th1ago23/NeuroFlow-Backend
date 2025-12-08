using Application.DTO.Common;

namespace Application.DTO.Patients;

public sealed record PatientDetailDto(
    Guid Id,
    NameDto Name,
    DateOnly BirthDate,
    GenderDto Gender,
    AddressDto? Address,
    Guid? OwnerProfessionalId,
    bool IsActive
);
