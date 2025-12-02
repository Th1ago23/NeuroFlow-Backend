using Application.DTO.Auth;
using Application.DTO.Patients;
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

    public AuthService(IJwtProvider jwtProvider,IPasswordHasher passwordHasher,IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _userRepository = unitOfWork.Users;
        _patientRepository = unitOfWork.Patients;
        _profileRepository = unitOfWork.ProfessionalProfiles;
        _patientInviteRepository = unitOfWork.PatientInvites;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository
            .FirstOrDefaultAsync(u => u.Email.Address == request.Email, ct);

        if (user is null)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var isValidPassword = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isValidPassword)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        user.UpdateLastLogin();

        await _unitOfWork.CommitAsync();

        var isVerified = user.ProfessionalProfile?.IsVerified ?? false;
        var isPremium = user.ProfessionalProfile?.IsPremium ?? false;

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

    public async Task<LoginResponse> RegisterProfessionalAsync(
        RegisterProfessionalRequest request,
        CancellationToken ct = default)
    {
        var existing = await _userRepository
            .FirstOrDefaultAsync(u => u.Email.Address == request.Email, ct);

        if (existing is not null)
            throw new InvalidOperationException("E-mail já está em uso.");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = request.ToUser(passwordHash);
        var profile = request.ToProfile(user.Id);

        await _userRepository.AddAsync(user, ct);
        await _profileRepository.AddAsync(profile, ct);

        await _unitOfWork.CommitAsync();

        var isVerified = profile.IsVerified;
        var isPremium = profile.IsPremium;

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
    public async Task<LoginResponse> RegisterPatientAsync(
        RegisterPatientRequest request,
        CancellationToken ct = default)
    {
        var existing = await _userRepository
            .FirstOrDefaultAsync(u => u.Email.Address == request.Email, ct);

        if (existing is not null)
            throw new InvalidOperationException("E-mail já está em uso.");

        var invite = await _patientInviteRepository
            .GetActiveByTokenAsync(request.InviteToken, ct);

        if (invite is null)
            throw new InvalidOperationException("Convite inválido ou expirado.");

        var ownerProfessionalId = invite.ProfessionalUserId;

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = request.ToUser(passwordHash);
        await _userRepository.AddAsync(user, ct);

        var patient = request.ToPatient(user.Id, ownerProfessionalId);
        await _patientRepository.AddAsync(patient, ct);

        invite.MarkAsUsed();
        _patientInviteRepository.Update(invite);

        await _unitOfWork.CommitAsync();

        var jwt = await _jwtProvider.GenerateTokenAsync(
            userId: user.Id,
            email: user.Email.Address,
            role: user.Role.ToString(),
            isVerifiedProfessional: false,
            isPremium: false,
            ct: ct);

        return new LoginResponse(
            AccessToken: jwt.AccessToken,
            ExpiresAt: jwt.ExpiresAt,
            User: user.ToSummaryDto()
        );
    }
    public async Task<PatientInviteDto> CheckInviteAsync(Guid token, CancellationToken ct = default)
    {
        var invite = await _patientInviteRepository.GetActiveByTokenAsync(token, ct);

        if (invite is null)
            throw new InvalidOperationException("Convite inválido ou expirado.");

        return new PatientInviteDto(
            Token: invite.Token,
            ProfessionalUserId: invite.ProfessionalUserId,
            ExpiresAt: invite.ExpiresAt,
            IsUsed: invite.IsUsed
        );
    }
}
