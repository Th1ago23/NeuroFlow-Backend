using Application.Common.Responses;
using Application.DTO.Patients;
using Application.Interfaces.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue("uid")!);

    private string GetUserRole() => User.FindFirstValue("role")!;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request,CancellationToken ct)
    {
        try
        {
            var userId = GetUserId();
            var id = await _patientService.CreateAsync(request, userId, ct);

            return Ok(ApiResponse<Guid>.Ok(id, "Paciente criado com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<Guid>.Fail(ex.Message));
        }
    }

    [HttpGet("me")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        try
        {
            var userId = GetUserId();
            var dto = await _patientService.GetMyProfileAsync(userId, ct);

            return Ok(ApiResponse<PatientDetailDto>.Ok(dto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PatientDetailDto>.Fail(ex.Message));
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> GetMyPatients(CancellationToken ct)
    {
        try
        {
            var professionalId = GetUserId();
            var list = await _patientService.GetByProfessionalAsync(professionalId, ct);

            return Ok(ApiResponse<IEnumerable<PatientListItemDto>>.Ok(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<PatientListItemDto>>.Fail(ex.Message));
        }
    }

    [HttpGet("{patientId:guid}")]
    [Authorize(Roles = "Admin,Professional,Assistent,Patient")]
    public async Task<IActionResult> GetById(Guid patientId, CancellationToken ct)
    {
        try
        {
            var requesterId = GetUserId();
            var role = GetUserRole();

            var dto = await _patientService.GetByIdAsync(patientId, requesterId, role, ct);

            return Ok(ApiResponse<PatientDetailDto>.Ok(dto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PatientDetailDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{patientId:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid patientId,[FromBody] UpdatePatientRequest request,CancellationToken ct)
    {
        if (patientId != request.Id)
            return BadRequest(ApiResponse<string>.Fail("Id da rota difere do corpo da requisição."));

        try
        {
            var requesterId = GetUserId();
            var role = GetUserRole();

            await _patientService.UpdateAsync(request, requesterId, role, ct);

            return Ok(ApiResponse<string>.Ok(null, "Paciente atualizado com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPost("{patientId:guid}/unassign")]
    [Authorize(Roles = "Admin,Professional")]
    public async Task<IActionResult> Unassign(Guid patientId, CancellationToken ct)
    {
        try
        {
            var professionalId = GetUserId();

            await _patientService.UnassignProfessionalAsync(patientId, professionalId, ct);

            return Ok(ApiResponse<string>.Ok(null,
                "Paciente desassociado do profissional. Dados serão mantidos por até 1 ano."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }
}
