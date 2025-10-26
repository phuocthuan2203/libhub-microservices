using LibHub.UserService.Application.DTOs;
using LibHub.UserService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibHub.UserService.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IdentityApplicationService _identityService;

    public UsersController(IdentityApplicationService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerDto)
    {
        try
        {
            var userDto = await _identityService.RegisterUserAsync(registerDto);
            return CreatedAtAction(nameof(GetUserById), new { id = userDto.UserId }, userDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during registration." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        try
        {
            var tokenDto = await _identityService.LoginUserAsync(loginDto);
            return Ok(tokenDto);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login." });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            if (currentUserId != id && !IsAdmin())
            {
                return Forbid();
            }

            var userDto = await _identityService.GetUserByIdAsync(id);
            return Ok(userDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving user." });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }
}
