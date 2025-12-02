using Application.Common.Responses;
using Application.DTO.Auth;
using Application.DTO.Patients;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    [HttpPost("register/professional")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterProfessional(
        [FromBody] RegisterProfessionalRequest request,
        CancellationToken ct)
    {
        var result = await _authService.RegisterProfessionalAsync(request, ct);
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    [HttpPost("register/patient")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterPatient(
        [FromBody] RegisterPatientRequest request,
        CancellationToken ct)
    {
        var result = await _authService.RegisterPatientAsync(request, ct);
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    [HttpGet("invite/{token:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckInvite(Guid token, CancellationToken ct)
    {
        var invite = await _authService.CheckInviteAsync(token, ct);
        return Ok(ApiResponse<PatientInviteDto>.Ok(invite));
    }
}
