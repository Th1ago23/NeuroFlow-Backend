using Application.DTO.Auth;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<LoginResponse> RegisterProfessionalAsync(RegisterProfessionalRequest request,CancellationToken ct = default);
    Task<LoginResponse> RegisterPatientAsync(RegisterPatientRequest request,CancellationToken ct = default);
}
