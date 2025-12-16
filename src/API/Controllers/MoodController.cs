using Application.Common.Responses;
using Application.DTO.Mood;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoodController : ControllerBase
{
    private readonly IMoodEntryService _service;

    public MoodController(IMoodEntryService service)
    {
        _service = service;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue("uid")!);

    private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role)!;

    [HttpPost]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> Create([FromBody] CreateMoodEntryRequest request,CancellationToken ct)
    {
        var patientUserId = GetUserId();

        var mood = await _service.CreateAsync(request, patientUserId, ct);

        return Ok(ApiResponse<MoodEntryDto>.Ok(mood, "Humor registrado com sucesso."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> Update(Guid id,[FromBody] UpdateMoodEntryRequest request,CancellationToken ct)
    {
        var patientUserId = GetUserId();

        var mood = await _service.UpdateAsync(id, request, patientUserId, ct);

        return Ok(ApiResponse<MoodEntryDto>.Ok(mood, "Registro de humor atualizado."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var patientUserId = GetUserId();

        await _service.DeleteAsync(id, patientUserId, ct);

        return Ok(ApiResponse.Ok("Registro de humor removido."));
    }

    [HttpGet("recent/{patientId:guid}")]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> GetRecent(Guid patientId,[FromQuery] int limit = 10,CancellationToken ct = default)
    {
        var requesterUserId = GetUserId();
        var requesterRole = GetUserRole();

        var moods = await _service.GetRecentAsync(patientId, limit, requesterUserId, requesterRole, ct);

        return Ok(ApiResponse<IEnumerable<MoodEntryDto>>.Ok(moods));
    }


    [HttpGet("stats/{patientId:guid}")]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> GetStats(Guid patientId, CancellationToken ct)
    {
        var requesterUserId = GetUserId();
        var requesterRole = GetUserRole();

        var stats = await _service.GetStatsAsync(patientId, requesterUserId, requesterRole, ct);

        return Ok(ApiResponse<MoodStatsDto>.Ok(stats));
    }
    [HttpGet("patients/{patientId:guid}/range")]
    public async Task<IActionResult> GetByDateRangeAsync(Guid patientId,[FromQuery] DateTime start,[FromQuery] DateTime end,CancellationToken ct)
    {
        var requesterId = Guid.Parse(User.FindFirst("sub")!.Value);
        var requesterRole = User.FindFirst("role")!.Value;

        if (start > end) return BadRequest("A data inicial não pode ser maior que a data final.");

        var entries = await _service
            .GetByDateRangeAsync(patientId, start, end, requesterId, requesterRole, ct);

        return Ok(entries);
    }
}
