using Application.DTO.Auth;
using Application.Interfaces.Services;
using Application.Mappings.Users;
using Domain.Entities;
using Domain.Entities.Profiles;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Security;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IProfessionalProfileRepository _profileRepository;
    private readonly IPatientInviteRepository _patientInviteRepository;

    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        // Repositórios expostos pelo UoW
        _userRepository = unitOfWork.Users;
        _patientRepository = unitOfWork.Patients;
        _profileRepository = unitOfWork.ProfessionalProfiles;
        _patientInviteRepository = unitOfWork.PatientInvites;

        // Serviços
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        // 1. Buscar usuário por e-mail
        var user = await _userRepository
            .FirstOrDefaultAsync(u => u.Email.Address == request.Email, ct);

        if (user is null)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        // 2. Validar senha
        var isValidPassword = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isValidPassword)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        // 3. Atualizar último login
        user.UpdateLastLogin();

        await _unitOfWork.CommitAsync();

        // 4. Definir flags para o JWT (profissional verificado/premium)
        var isVerified = user.ProfessionalProfile?.IsVerified ?? false;
        var isPremium = user.ProfessionalProfile?.IsPremium ?? false;

        // 5. Gerar token com auditoria
        var jwt = await _jwtProvider.GenerateTokenAsync(
            userId: user.Id,
            email: user.Email.Address,
            role: user.Role.ToString(),
            isVerifiedProfessional: isVerified,
            isPremium: isPremium,
            ct: ct);

        // 6. Montar resposta
        return new LoginResponse(
            AccessToken: jwt.AccessToken,
            ExpiresAt: jwt.ExpiresAt,
            User: user.ToSummaryDto()
        );
    }

    // --------------- REGISTER PROFESSIONAL ---------------

    public async Task<LoginResponse> RegisterProfessionalAsync(
        RegisterProfessionalRequest request,
        CancellationToken ct = default)
    {
        // 1. Verificar se o e-mail já está em uso
        var existing = await _userRepository
            .FirstOrDefaultAsync(u => u.Email.Address == request.Email, ct);

        if (existing is not null)
            throw new InvalidOperationException("E-mail já está em uso.");

        // 2. Hash da senha
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 3. Criar User e ProfessionalProfile via mapeamentos
        var user = request.ToUser(passwordHash);
        var profile = request.ToProfile(user.Id);

        // 4. Persistir
        await _userRepository.AddAsync(user, ct);
        await _profileRepository.AddAsync(profile, ct);

        await _unitOfWork.CommitAsync();

        // 5. Flags para o token
        var isVerified = profile.IsVerified;
        var isPremium = profile.IsPremium;

        // 6. Gerar token com auditoria
        var jwt = await _jwtProvider.GenerateTokenAsync(
            userId: user.Id,
            email: user.Email.Address,
            role: user.Role.ToString(),
            isVerifiedProfessional: isVerified,
            isPremium: isPremium,
            ct: ct);

        return new LoginResponse(
            AccessToken: jwt.AccessToken,
            ExpiresAt: jwt.ExpiresAt,
            User: user.ToSummaryDto()
        );
    }

    // ---------------- REGISTER PATIENT -------------------

    public async Task<LoginResponse> RegisterPatientAsync(
        RegisterPatientRequest request,
        CancellationToken ct = default)
    {
        // 1. Verificar se e-mail já está em uso
        var existing = await _userRepository
            .FirstOrDefaultAsync(u => u.Email.Address == request.Email, ct);

        if (existing is not null)
            throw new InvalidOperationException("E-mail já está em uso.");

        // 2. Validar InviteToken e obter o profissional dono do paciente
        var invite = await _patientInviteRepository
            .GetActiveByTokenAsync(request.InviteToken, ct);

        if (invite is null)
            throw new InvalidOperationException("Convite inválido ou expirado.");

        var ownerProfessionalId = invite.ProfessionalUserId;

        // 3. Hash da senha
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 4. Criar User
        var user = request.ToUser(passwordHash);
        await _userRepository.AddAsync(user, ct);

        // 5. Criar Patient (vinculado ao profissional)
        var patient = request.ToPatient(user.Id, ownerProfessionalId);
        await _patientRepository.AddAsync(patient, ct);

        // 6. Marcar convite como usado
        invite.MarkAsUsed();
        _patientInviteRepository.Update(invite);

        // 7. Persistir mudanças (transação)
        await _unitOfWork.CommitAsync();

        // 8. Gerar JWT (paciente nunca é profissional)
        var jwt = await _jwtProvider.GenerateTokenAsync(
            userId: user.Id,
            email: user.Email.Address,
            role: user.Role.ToString(),
            isVerifiedProfessional: false,
            isPremium: false,
            ct: ct);

        // 9. Retornar LoginResponse
        return new LoginResponse(
            AccessToken: jwt.AccessToken,
            ExpiresAt: jwt.ExpiresAt,
            User: user.ToSummaryDto()
        );
    }

}
