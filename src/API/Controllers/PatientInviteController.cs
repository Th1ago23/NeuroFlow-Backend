
using Application.Common.Responses;
using Application.DTO.Patients;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;
[ApiController]
[Route("api/patient-invites")]
[Authorize(Roles = "Professional")]
public class PatientInviteController : ControllerBase
{
    private readonly IPatientInviteService _service;

    public PatientInviteController(IPatientInviteService service)
    {
        _service = service;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue("uid")!);

    [HttpPost]
    public async Task<IActionResult> CreateInvite (CancellationToken ct)
    {
        var professionalId = GetUserId();
        var invite = await _service.CreateInviteAsync(professionalId,ct);
        if (invite is null) return BadRequest(ApiResponse.Fail("Não foi possível criar um convite."));

        return Ok(ApiResponse<PatientInviteDto>.Ok(invite,"Convite criado com sucesso"));
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ValidateInvite(Guid token, CancellationToken ct)
    {
        var validation = await _service.ValidateInviteAsync(token, ct);

        return Ok(ApiResponse<PatientInviteDto>.Ok(validation,"Convite validado com sucesso"));
        
    }
}
