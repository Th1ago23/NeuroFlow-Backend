using Application.Common.Responses;
using Application.DTO.Common;
using Application.DTO.Users;
using Application.Interfaces.Users;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue("uid")!);

    private string GetUserRole() =>
        User.FindFirstValue("role")!;

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        try
        {
            var userId = GetUserId();
            var dto = await _userService.GetByIdAsync(userId);

            if (dto is null)
                return NotFound(ApiResponse<UserDetailDto>.Fail("Usuário não encontrado."));

            return Ok(ApiResponse<UserDetailDto>.Ok(dto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserDetailDto>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var dto = await _userService.GetByIdAsync(id);

            if (dto is null)
                return NotFound(ApiResponse<UserDetailDto>.Fail("Usuário não encontrado."));

            return Ok(ApiResponse<UserDetailDto>.Ok(dto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserDetailDto>.Fail(ex.Message));
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var list = await _userService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<UserSummaryDto>>.Ok(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<UserSummaryDto>>.Fail(ex.Message));
        }
    }

    [HttpPut("me/name")]
    [Authorize]
    public async Task<IActionResult> UpdateMyName([FromBody] NameDto nameDto)
    {
        try
        {
            var userId = GetUserId();
            await _userService.UpdateNameAsync(userId, nameDto);

            return Ok(ApiResponse<string>.Ok(null, "Nome atualizado com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("me/email")]
    [Authorize]
    public async Task<IActionResult> UpdateMyEmail([FromBody] EmailDto emailDto)
    {
        try
        {
            var userId = GetUserId();
            await _userService.UpdateEmailAsync(userId, emailDto.Address);

            return Ok(ApiResponse<string>.Ok(null, "E-mail atualizado com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("me/phone")]
    [Authorize]
    public async Task<IActionResult> UpdateMyPhone([FromBody] PhoneDto phoneDto)
    {
        try
        {
            var userId = GetUserId();
            await _userService.UpdatePhoneAsync(userId, phoneDto.Number);

            return Ok(ApiResponse<string>.Ok(null, "Telefone atualizado com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPatch("{id:guid}/disable")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Disable(Guid id)
    {
        try
        {
            await _userService.DisableAsync(id);
            return Ok(ApiResponse<string>.Ok(null, "Usuário desativado com sucesso."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }
}
