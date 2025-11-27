using Application.DTO.Auth;
using Application.DTO.Clinical;
using Application.DTO.Common;
using Application.DTO.Mood;
using Application.DTO.Patients;
using Application.DTO.Thoughts;
using Application.Mappings.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Mappings.Users;

public static class PatientMappings
{
    public static User ToUser(this RegisterPatientRequest dto, string passwordHash)
        => new(
            name: new Name(dto.Name.FirstName, dto.Name.LastName),
            email: new Email(dto.Email),
            passwordHash: passwordHash,
            role: Domain.Enums.UserRole.Patient,
            phone: dto.Phone,
            cpf: null
        );

    public static Patient ToPatient(this RegisterPatientRequest dto, Guid userId, Guid professionalId)
    {
        var patient = new Patient(
            name: new Name(dto.Name.FirstName, dto.Name.LastName),
            birthDate: dto.BirthDate,
            gender: new Gender(dto.Gender.Type, dto.Gender.CustomValue),
            ownerProfessionalId: professionalId,
            address: dto.Address?.ToValueObject()
        );

        patient.LinkUserAccount(userId);
        return patient;
    }


    public static PatientDetailDto ToDetailDto(this Patient p)
        => new(
            Id: p.Id,
            Name: new NameDto(p.Name.FirstName, p.Name.LastName),
            BirthDate: p.BirthDate,
            Gender: new GenderDto(p.Gender.Type, p.Gender.CustomValue),
            Address: p.Address?.ToDto(),
            OwnerProfessionalId: p.OwnerProfessionalId,
            IsActive: p.IsActive
        );

    public static PatientSummaryDto ToSummaryDto(this Patient p)
        => new(
            Id: p.Id,
            Name: new NameDto(p.Name.FirstName, p.Name.LastName),
            BirthDate: p.BirthDate,
            Gender: new GenderDto(p.Gender.Type, p.Gender.CustomValue),
            IsActive: p.IsActive
        );

    public static PatientListItemDto ToListItemDto(this Patient p)
        => new(
            Id: p.Id,
            Name: new NameDto(p.Name.FirstName, p.Name.LastName),
            IsActive: p.IsActive
        );

    public static PatientDashboardDto ToDashboardDto(
        this Patient patient,
        IEnumerable<MoodEntryDto> recentMoods,
        IEnumerable<ThoughtEntryDto> recentThoughts,
        IEnumerable<ClinicalNoteDto> recentNotes)
        => new(
            Patient: patient.ToDetailDto(),
            RecentMoods: recentMoods,
            RecentThoughts: recentThoughts,
            RecentNotes: recentNotes
        );
}
