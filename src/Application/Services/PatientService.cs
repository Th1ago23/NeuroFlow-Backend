using Application.DTO.Common;
using Application.DTO.Patients;
using Application.Interfaces.Patients;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepo;
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;

    public PatientService(IPatientRepository patientRepo,IUserRepository userRepo,IUnitOfWork uow)
    {
        _patientRepo = patientRepo;
        _userRepo = userRepo;
        _uow = uow;
    }

    public async Task<Guid> CreateAsync(CreatePatientRequest request,Guid userAccountId,CancellationToken ct)
    {
        var user = await _userRepo.GetById(userAccountId);
        if (user is null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (user.Role != UserRole.Patient)
            throw new UnauthorizedAccessException("Somente usuários pacientes podem criar um perfil de paciente.");

        var existing = await _patientRepo.GetByUserId(userAccountId, ct);
        if (existing is not null)
            throw new InvalidOperationException("Já existe um paciente vinculado a este usuário.");

        var professional = await _userRepo.GetById(request.OwnerProfessionalId);
        if (professional is null || professional.Role != UserRole.Professional)
            throw new InvalidOperationException("Profissional responsável inválido.");

        var name = new Name(request.Name.FirstName, request.Name.LastName);
        var gender = new Gender(request.Gender.Type, request.Gender.CustomValue);

        Address? address = request.Address is null
            ? null
            : new Address(
                request.Address.Number,
                request.Address.Street,
                request.Address.City,
                request.Address.State,
                request.Address.Country,
                request.Address.ZipCode
              );

        var patient = new Patient(
            name,
            request.BirthDate,
            gender,
            request.OwnerProfessionalId,
            address
        );

        patient.LinkUserAccount(userAccountId);

        await _patientRepo.AddAsync(patient, ct);
        await _uow.CommitAsync();

        return patient.Id;
    }

    public async Task<PatientDetailDto> GetByIdAsync(Guid patientId,Guid requesterId,string requesterRole,CancellationToken ct)
    {
        var patient = await _patientRepo.GetByIdAsync(patientId, ct);
        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        if (requesterRole == "Patient")
        {
            if (patient.UserAccountId != requesterId)
                throw new UnauthorizedAccessException("Você só pode acessar seu próprio perfil.");
        }
        else if (requesterRole == "Professional")
        {
            if (patient.OwnerProfessionalId != requesterId)
                throw new UnauthorizedAccessException("Você não pode acessar pacientes de outro profissional.");
        }

        return ToDetailDto(patient);
    }

    public async Task<PatientDetailDto> GetMyProfileAsync(Guid userAccountId,CancellationToken ct)
    {
        var patient = await _patientRepo.GetByUserId(userAccountId, ct);
        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado para este usuário.");

        return ToDetailDto(patient);
    }

    public async Task<IEnumerable<PatientListItemDto>> GetByProfessionalAsync(Guid professionalId,CancellationToken ct)
    {
        var professional = await _userRepo.GetById(professionalId);
        if (professional is null || professional.Role != UserRole.Professional)
            throw new UnauthorizedAccessException("Apenas profissionais podem listar seus pacientes.");

        var patients = await _patientRepo.GetByProfessionalIdAsync(professionalId);

        return patients
            .Select(p => new PatientListItemDto(
                p.Id,
                new NameDto(p.Name.FirstName, p.Name.LastName),
                p.IsActive
            ));
    }

    public async Task UpdateAsync(UpdatePatientRequest request,Guid requesterId,string requesterRole,CancellationToken ct)
    {
        var patient = await _patientRepo.GetByIdAsync(request.Id, ct);
        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        var name = new Name(request.Name.FirstName, request.Name.LastName);
        var gender = new Gender(request.Gender.Type, request.Gender.CustomValue);
        Address? address = request.Address is null
            ? null
            : new Address(
                request.Address.Number,
                request.Address.Street,
                request.Address.City,
                request.Address.State,
                request.Address.Country,
                request.Address.ZipCode
              );

        if (requesterRole == "Patient")
        {
            patient.UpdateSensitiveData(name, address);
        }
        else if (requesterRole == "Professional")
        {
            patient.UpdateSensitiveData(name, address);

            if (request.IsActive == false)
                patient.Deactivate();
        }
        else
        {
            throw new UnauthorizedAccessException("Perfil de usuário não autorizado a atualizar paciente.");
        }

        _patientRepo.Update(patient);
        await _uow.CommitAsync();
    }

    public async Task UnassignProfessionalAsync(Guid patientId,Guid professionalId,CancellationToken ct)
    {
        var patient = await _patientRepo.GetByIdAsync(patientId, ct);
        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        if (patient.OwnerProfessionalId != professionalId)
            throw new UnauthorizedAccessException("Você não é o profissional responsável por este paciente.");

        patient.UnassignProfessional();

        _patientRepo.Update(patient);
        await _uow.CommitAsync();
    }


    private static PatientDetailDto ToDetailDto(Patient p)
    {
        var nameDto = new NameDto(p.Name.FirstName, p.Name.LastName);
        var genderDto = new GenderDto(p.Gender.Type, p.Gender.CustomValue);

        AddressDto? addressDto = p.Address is null
            ? null
            : new AddressDto(
                p.Address.Number,
                p.Address.Street,
                p.Address.City,
                p.Address.State,
                p.Address.Country,
                p.Address.ZipCode
              );

        return new PatientDetailDto(
            p.Id,
            nameDto,
            p.BirthDate,
            genderDto,
            addressDto,
            p.OwnerProfessionalId ?? Guid.Empty,
            p.IsActive
        );
    }
}
