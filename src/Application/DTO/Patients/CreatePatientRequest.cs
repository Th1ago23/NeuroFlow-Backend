using Application.DTO.Common;

namespace Application.DTO.Patients;

public sealed record CreatePatientRequest(
    NameDto Name,
    DateOnly BirthDate,
    GenderDto Gender,
    Guid OwnerProfessionalId,
    AddressDto? Address
);
