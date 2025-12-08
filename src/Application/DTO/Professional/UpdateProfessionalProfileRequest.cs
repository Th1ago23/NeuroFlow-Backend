using Application.DTO.Common;

namespace Application.DTO.Professional;

public sealed record UpdateProfessionalProfileRequest(string Speciality,AddressDto Address);
