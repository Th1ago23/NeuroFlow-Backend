using Application.Common.Responses;
using Application.DTO.Homework;
using Application.Interfaces.Homework;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/homework")]
public class HomeworkController : ControllerBase
{
    private readonly IHomeworkService _homeworkService;
    private readonly IPatientRepository _patientRepo;

    public HomeworkController(
        IHomeworkService homeworkService,
        IPatientRepository patientRepo)
    {
        _homeworkService = homeworkService;
        _patientRepo = patientRepo;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue("sub")!);

    private string GetUserRole() =>
        User.FindFirstValue("role")!;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateHomeworkRequest request)
    {
        try
        {
            var userId = GetUserId();
            var role = GetUserRole();

            if (role != "Professional")
                return Forbid();

            // Regra de domínio está no service
            var id = await _homeworkService.CreateAsync(request, userId);

            return Ok(ApiResponse<Guid>.Ok(id, "Tarefa criada com sucesso."));
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
                return NotFound(ApiResponse<IEnumerable<HomeworkDto>>.Fail("Paciente não encontrado."));

            if (role == "Patient" && patient.UserAccountId != userId)
                return Forbid();

            if (role == "Professional" && patient.OwnerProfessionalId != userId)
                return Forbid();

            var list = await _homeworkService.GetByPatientAsync(patientId);

            return Ok(ApiResponse<IEnumerable<HomeworkDto>>.Ok(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<HomeworkDto>>.Fail(ex.Message));
        }
    }

    [HttpGet("professional")]
    [Authorize]
    public async Task<IActionResult> GetByProfessional()
    {
        try
        {
            var userId = GetUserId();
            var role = GetUserRole();

            if (role != "Professional")
                return Forbid();

            var list = await _homeworkService.GetByProfessionalAsync(userId);

            return Ok(ApiResponse<IEnumerable<HomeworkDto>>.Ok(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<HomeworkDto>>.Fail(ex.Message));
        }
    }

    [HttpPatch("{id:guid}/done")]
    [Authorize]
    public async Task<IActionResult> MarkAsDone(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var role = GetUserRole();

            if (role != "Patient")
                return Forbid();

            await _homeworkService.MarkAsDoneAsync(id, userId);

            return Ok(ApiResponse<string>.Ok(null, "Tarefa marcada como concluída."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHomeworkRequest request)
    {
        try
        {
            var userId = GetUserId();
            var role = GetUserRole();

            if (role != "Professional")
                return Forbid();

            await _homeworkService.UpdateAsync(id, request, userId);

            return Ok(ApiResponse<string>.Ok(null, "Tarefa atualizada com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var role = GetUserRole();

            if (role != "Professional")
                return Forbid();

            await _homeworkService.DeleteAsync(id, userId);

            return Ok(ApiResponse<string>.Ok(null, "Tarefa removida com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }
}
