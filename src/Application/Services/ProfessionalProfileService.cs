using Application.DTO.Common;
using Application.DTO.Professional;
using Application.Interfaces;
using Application.Interfaces.Http;
using Application.Interfaces.Logging;
using Application.Interfaces.Professional;
using Domain.Entities.Profiles;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.Services;

public class ProfessionalProfileService : IProfessionalProfileService
{
    private readonly IProfessionalProfileRepository _profileRepo;
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;
    private readonly IProfessionalDocumentVerifier _documentVerifier;
    private readonly IAppLogger<ProfessionalProfileService> _logger;

    public ProfessionalProfileService(IAppLogger<ProfessionalProfileService> logger, IProfessionalDocumentVerifier documentVerifier,IProfessionalProfileRepository profileRepo,IUserRepository userRepo,IUnitOfWork uow)
    {
        _logger = logger;
        _documentVerifier = documentVerifier;
        _profileRepo = profileRepo;
        _userRepo = userRepo;
        _uow = uow;
    }

    public async Task<ProfessionalProfileDto> CreateAsync(Guid userId, CreateProfessionalProfileRequest request)
    {
        var user = await _userRepo.GetById(userId)
                   ?? throw new KeyNotFoundException("Usuário não encontrado.");

        if (user.Role != UserRole.Professional)
            throw new UnauthorizedAccessException("Apenas profissionais podem criar perfis.");

        if (user.ProfessionalProfile is not null)
            throw new InvalidOperationException("Usuário já possui perfil profissional.");

        var existingDoc = await _profileRepo.GetByDocumentAsync(request.DocumentNumber);
        if (existingDoc is not null)
            throw new InvalidOperationException("Documento já cadastrado para outro profissional.");

        var address = new Address(
            request.Address.Number,
            request.Address.Street,
            request.Address.City,
            request.Address.State,
            request.Address.Country,
            request.Address.ZipCode
        );

        var profile = new ProfessionalProfile(
            userId,
            request.DocumentNumber,
            request.Speciality,
            address
        );

        await _profileRepo.AddAsync(profile);
        await _uow.CommitAsync();

        user.AttachProfessionalProfile(profile);
        _userRepo.Update(user);
        await _uow.CommitAsync();

        return ToDto(profile);
    }

    public async Task<ProfessionalProfileDto?> GetByUserIdAsync(Guid userId)
    {
        var user = await _userRepo.GetById(userId);
        if (user?.ProfessionalProfile is null)
            return null;

        return ToDto(user.ProfessionalProfile);
    }

    public async Task<ProfessionalProfileDto?> GetByDocumentAsync(string document)
    {
        var profile = await _profileRepo.GetByDocumentAsync(document);
        return profile is null ? null : ToDto(profile);
    }

    public async Task UpdateAsync(Guid userId, UpdateProfessionalProfileRequest request)
    {
        var user = await _userRepo.GetById(userId)
                   ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var profile = user.ProfessionalProfile
                      ?? throw new InvalidOperationException("Usuário não possui perfil profissional.");

        var address = new Address(
            request.Address.Number,
            request.Address.Street,
            request.Address.City,
            request.Address.State,
            request.Address.Country,
            request.Address.ZipCode
        );

        profile.UpdateProfile(request.Speciality, address);

        _userRepo.Update(user);
        await _uow.CommitAsync();
    }

    public async Task VerifyAsync(Guid userId, VerifyProfessionalDocumentRequest request)
    {
        _logger.LogInformation("Iniciando verificação do profissional {UserId}", userId);

        var user = await _userRepo.GetById(userId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var profile = user.ProfessionalProfile
                      ?? throw new InvalidOperationException("Usuário não possui perfil profissional.");

        var result = await _documentVerifier.VerifyAsync(profile.DocumentNumber);

        if (!result.IsValid)
        {
            _logger.LogWarning("Falha na verificação do documento {Document}", profile.DocumentNumber);
            throw new InvalidOperationException("Documento inválido.");
        }

        profile.Verify(request.DocumentFileUrl ?? "");

        _logger.LogInformation("Documento verificado com sucesso {Document}", profile.DocumentNumber);

        _userRepo.Update(user);
        await _uow.CommitAsync();
    }

    //public async Task UpgradeToPremiumAsync(Guid userId)
    //{
    //    var user = await _userRepo.GetById(userId)
    //               ?? throw new KeyNotFoundException("Usuário não encontrado.");

    //    var profile = user.ProfessionalProfile
    //                  ?? throw new InvalidOperationException("Usuário não possui perfil profissional.");



    //    _userRepo.Update(user);
    //    await _uow.CommitAsync();
    //}

    private static ProfessionalProfileDto ToDto(ProfessionalProfile profile)
    {
        var address = new AddressDto(
            profile.Address.Number,
            profile.Address.Street,
            profile.Address.City,
            profile.Address.State,
            profile.Address.Country,
            profile.Address.ZipCode
        );

        return new ProfessionalProfileDto(
            profile.Id,
            profile.UserId,
            profile.DocumentNumber,
            profile.Speciality,
            address,
            profile.IsVerified,
            profile.DocumentFileUrl,
            profile.IsPremium
        );
    }
}
