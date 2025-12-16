using Application.Common.Responses;
using Application.DTO.Clinical;
using Application.Interfaces.Clinical;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/clinical-assessments")]
public class ClinicalAssessmentController : ControllerBase
{
    private readonly IClinicalAssessmentService _assessmentService;
    private readonly IPatientRepository _patientRepo;

    public ClinicalAssessmentController(
        IClinicalAssessmentService assessmentService,
        IPatientRepository patientRepo)
    {
        _assessmentService = assessmentService;
        _patientRepo = patientRepo;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue("sub")!);

    private string GetUserRole() =>
        User.FindFirstValue("role")!;

    [HttpPost]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> Create([FromBody] CreateClinicalAssessmentRequest request)
    {
        try
        {
            var role = GetUserRole();
            var requesterId = GetUserId();

            if (requesterId != request.ProfessionalUserId)
                return Forbid("Você só pode registrar avaliações como você mesmo.");

            var id = await _assessmentService.CreateAsync(request);

            return Ok(ApiResponse<Guid>.Ok(id, "Avaliação clínica registrada."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<Guid>.Fail(ex.Message));
        }
    }

    [HttpGet("patient/{patientId:guid}")]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        try
        {
            var requesterId = GetUserId();
            var role = GetUserRole();

            var patient = await _patientRepo.GetByIdAsync(patientId);
            if (patient is null)
                return NotFound(ApiResponse<IEnumerable<ClinicalAssessmentDto>>
                    .Fail("Paciente não encontrado."));

            if (role == "Patient" && patient.UserAccountId != requesterId)
                return Forbid("Você só pode visualizar suas próprias avaliações clínicas.");

            if (role == "Professional" && patient.OwnerProfessionalId != requesterId)
                return Forbid("Você não pode acessar avaliações de pacientes de outro profissional.");

            var result = await _assessmentService.GetByPatientAsync(patientId);

            return Ok(ApiResponse<IEnumerable<ClinicalAssessmentDto>>.Ok(
                result,
                "Avaliações clínicas recuperadas."
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<ClinicalAssessmentDto>>.Fail(ex.Message));
        }
    }

    [HttpPut("{assessmentId:guid}")]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> Update(Guid assessmentId,[FromBody] UpdateClinicalAssessmentRequest request)
    {
        try
        {
            var role = GetUserRole();

            await _assessmentService.UpdateAsync(assessmentId, request);

            return Ok(ApiResponse<string>.Ok(null, "Avaliação clínica atualizada."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpDelete("{assessmentId:guid}")]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> Delete(Guid assessmentId)
    {
        try
        {
            var role = GetUserRole();

            await _assessmentService.DeleteAsync(assessmentId);

            return Ok(ApiResponse<string>.Ok(null, "Avaliação clínica removida."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }
}
