using Application.Common.Responses;
using Application.DTO.Clinical;
using Application.Interfaces.Clinical;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/clinical-notes")]
[Authorize(Roles = "Professional")]
public class ClinicalNotesController : ControllerBase
{
    private readonly IClinicalNoteService _service;

    public ClinicalNotesController(IClinicalNoteService service)
    {
        _service = service;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue("uid")!);

    private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role)!;

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateClinicalNoteRequest request,
        CancellationToken ct)
    {
        var requesterId = GetUserId();
        var requesterRole = GetUserRole();

        var id = await _service.CreateAsync(request, requesterId, requesterRole);

        return Ok(ApiResponse<Guid>.Ok(id, "Nota clínica criada com sucesso."));
    }

    [HttpGet("{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        var requesterId = GetUserId();
        var requesterRole = GetUserRole();

        var notes = await _service.GetByPatientAsync(patientId, requesterId, requesterRole);

        return Ok(ApiResponse<IEnumerable<ClinicalNoteDto>>.Ok(
            notes,
            "Notas clínicas obtidas com sucesso."
        ));
    }

    [HttpPatch("{noteId:guid}")]
    public async Task<IActionResult> Update(Guid noteId, [FromBody] string newText)
    {
        var requesterId = GetUserId();
        var requesterRole = GetUserRole();

        await _service.UpdateAsync(noteId, newText, requesterId, requesterRole);

        return Ok(ApiResponse<string>.Ok("Nota clínica atualizada com sucesso."));
    }

    [HttpDelete("{noteId:guid}")]
    public async Task<IActionResult> Delete(Guid noteId)
    {
        var requesterId = GetUserId();
        var requesterRole = GetUserRole();

        await _service.DeleteAsync(noteId, requesterId, requesterRole);

        return Ok(ApiResponse<string>.Ok("Nota clínica removida com sucesso."));
    }
}
