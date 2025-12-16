using Application.DTO.Appointments;
using Application.Interfaces.Appointments;
using Application.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain.Interfaces.Repositories;

namespace API.Controllers;

[ApiController]

[Route("api/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue("sub")!);

    private string GetUserRole() =>
        User.FindFirstValue("role")!;

    [HttpPost]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        var userId = GetUserId();
        var role = GetUserRole();

        if (role != "Professional")
            return Forbid();

        if (userId != request.ProfessionalUserId)
            return Forbid();

        try
        {
            var id = await _appointmentService.CreateAsync(request);
            return Ok(ApiResponse<Guid>.Ok(id, "Consulta marcada com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<Guid>.Fail(ex.Message, "APPOINTMENT_CREATION_FAILED"));
        }
    }

    [HttpGet("professional/upcoming")]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> GetUpcomingForProfessional()
    {
        var userId = GetUserId();
        var role = GetUserRole();


        var data = await _appointmentService.GetUpcomingByProfessionalAsync(userId);

        return Ok(ApiResponse<IEnumerable<AppointmentDto>>.Ok(
            data,
            "Consultas futuras do profissional recuperadas."
        ));
    }
    [HttpGet("patient/upcoming")]
    [Authorize(Roles ="Patient")]
    public async Task<IActionResult> GetUpcomingForPatient()
    {
        var userId = GetUserId();
        var role = GetUserRole();

        if (role != "Patient")
            return Forbid();

        var data = await _appointmentService.GetUpcomingByPatientAsync(userId);

        return Ok(ApiResponse<IEnumerable<AppointmentDto>>.Ok(
            data,
            "Consultas futuras do paciente recuperadas."
        ));
    }

    [HttpPatch("{appointmentId:guid}/cancel")]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> Cancel(Guid appointmentId)
    {
        try
        {
            await _appointmentService.CancelAsync(appointmentId);
            return Ok(ApiResponse<string>.Ok(null, "Consulta cancelada com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message, "APPOINTMENT_CANCEL_FAILED"));
        }
    }

    [HttpPatch("{appointmentId:guid}/complete")]
    [Authorize(Roles = "Admin,Professional,Assistent")]
    public async Task<IActionResult> Complete(Guid appointmentId)
    {
        try
        {
            await _appointmentService.CompleteAsync(appointmentId);
            return Ok(ApiResponse<string>.Ok(null, "Consulta concluída com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message, "APPOINTMENT_COMPLETE_FAILED"));
        }
    }
}
