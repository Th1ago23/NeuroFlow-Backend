using Application.DTO.Common;

namespace Application.DTO.Professional;

public sealed record ProfessionalProfileDto(Guid Id,Guid UserId,string DocumentNumber,string Speciality,AddressDto Address,bool IsVerified,string? DocumentFileUrl,bool IsPremium);
