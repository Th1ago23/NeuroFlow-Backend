using Application.Common.Responses;
using Application.DTO.Professional;
using Application.Interfaces.Professional;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/professional-profile")]
public class ProfessionalProfileController : ControllerBase
{
    private readonly IProfessionalProfileService _service;

    public ProfessionalProfileController(IProfessionalProfileService service)
    {
        _service = service;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue("uid")!);

    private string GetRole() =>
        User.FindFirstValue("role")!;

    [Authorize(Roles = "Professional")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProfessionalProfileRequest request)
    {
        try
        {
            var userId = GetUserId();
            var dto = await _service.CreateAsync(userId, request);

            return Ok(ApiResponse<ProfessionalProfileDto>.Ok(
                dto,
                "Perfil profissional criado com sucesso."
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProfessionalProfileDto>.Fail(ex.Message));
        }
    }

    [Authorize(Roles = "Professional")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userId = GetUserId();
            var dto = await _service.GetByUserIdAsync(userId);

            if (dto is null)
                return NotFound(ApiResponse<ProfessionalProfileDto>.Fail("Perfil não encontrado."));

            return Ok(ApiResponse<ProfessionalProfileDto>.Ok(dto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProfessionalProfileDto>.Fail(ex.Message));
        }
    }

    [Authorize(Roles = "Professional")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProfessionalProfileRequest request)
    {
        try
        {
            var userId = GetUserId();
            await _service.UpdateAsync(userId, request);

            return Ok(ApiResponse<string>.Ok(
                null,
                "Perfil atualizado com sucesso."
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [Authorize(Roles = "Professional")]
    [EnableRateLimiting("document-verification")]
    [HttpPatch("verify")]
    public async Task<IActionResult> VerifyDocument([FromBody] VerifyProfessionalDocumentRequest request)
    {
        try
        {
            var userId = GetUserId();
            await _service.VerifyAsync(userId, request);

            return Ok(ApiResponse<string>.Ok(
                null,
                "Documento verificado com sucesso."
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    //[Authorize(Roles = "Professional")]
    //[HttpPatch("upgrade")]
    //public async Task<IActionResult> UpgradeToPremium()
    //{
    //    try
    //    {
    //        var userId = GetUserId();
    //        await _service.UpgradeToPremiumAsync(userId);

    //        return Ok(ApiResponse<string>.Ok(
    //            null,
    //            "Plano premium ativado com sucesso."
    //        ));
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ApiResponse<string>.Fail(ex.Message));
    //    }
    //}

    [AllowAnonymous]
    [HttpGet("document/{documentNumber}")]
    public async Task<IActionResult> GetByDocument(string documentNumber)
    {
        try
        {
            var dto = await _service.GetByDocumentAsync(documentNumber);

            if (dto is null)
                return NotFound(ApiResponse<ProfessionalProfileDto>.Fail("Profissional não encontrado."));

            return Ok(ApiResponse<ProfessionalProfileDto>.Ok(dto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProfessionalProfileDto>.Fail(ex.Message));
        }
    }
}
