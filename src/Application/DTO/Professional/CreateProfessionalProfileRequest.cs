using Application.DTO.Common;

namespace Application.DTO.Professional;

public sealed record CreateProfessionalProfileRequest(string DocumentNumber,string Speciality,AddressDto Address);
