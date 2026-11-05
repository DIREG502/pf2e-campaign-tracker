using CampaignTracker.Api.Auth;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignTracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);

        if (response is null)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Email already registered",
                detail: "An account with this email address already exists.");
        }

        return CreatedAtAction(nameof(Me), null, response);
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response is null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid credentials",
                detail: "Email or password is incorrect.");
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var user = await _authService.GetUserAsync(User.GetUserId());
        return user is null ? NotFound() : Ok(user);
    }
}
