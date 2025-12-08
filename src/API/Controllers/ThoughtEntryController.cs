using Application.Common.Responses;
using Application.DTO.Thoughts;
using Application.Interfaces.Thoughts;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/thoughts")]
public class ThoughtEntryController : ControllerBase
{
    private readonly IThoughtEntryService _service;
    private readonly IPatientRepository _patientRepo;

    public ThoughtEntryController(
        IThoughtEntryService service,
        IPatientRepository patientRepo)
    {
        _service = service;
        _patientRepo = patientRepo;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue("uid")!);

    private string GetUserRole() =>
        User.FindFirstValue("role")!;

    [HttpPost]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> Create([FromBody] CreateThoughtEntryRequest request)
    {
        try
        {
            var userId = GetUserId();
            var patient = await _patientRepo.GetByUserId(userId, default);

            if (patient is null || patient.Id != request.PatientId)
                return Forbid("Você só pode registrar pensamentos para si mesmo.");

            var id = await _service.CreateAsync(request);

            return Ok(ApiResponse<Guid>.Ok(id, "Pensamento registrado com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<Guid>.Fail(ex.Message));
        }
    }

    [HttpGet("patient/{patientId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        try
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var patient = await _patientRepo.GetByIdAsync(patientId);
            if (patient is null)
                return NotFound(ApiResponse<IEnumerable<ThoughtEntryDto>>.Fail("Paciente não encontrado."));

            if (role == "Patient")
            {
                if (patient.UserAccountId != userId)
                    return Forbid();

                var list = await _service.GetByPatientAsync(patientId);
                return Ok(ApiResponse<IEnumerable<ThoughtEntryDto>>.Ok(list));
            }

            if (role == "Professional")
            {
                if (patient.OwnerProfessionalId != userId)
                    return Forbid();

                var list = await _service.GetByPatientAsync(patientId);

                var visible = list.Where(t => t.IsVisibleToProfessional);

                return Ok(ApiResponse<IEnumerable<ThoughtEntryDto>>.Ok(visible));
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<ThoughtEntryDto>>.Fail(ex.Message));
        }
    }
}
