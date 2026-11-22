using EQIMTANVIZITI.DependencyInjection.Users;
using EQIMTANVIZITI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EQIMTANVIZITI.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class UserController : ControllerBase
{
    private readonly IUserService _us;
    public UserController(IUserService us) => _us = us;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterRequest request)
    {
        var result = await _us.Register(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public IActionResult LoginUser([FromQuery] string? email, [FromQuery] string? password, [FromBody] LoginRequest? body = null)
    {
        var e = !string.IsNullOrWhiteSpace(email) ? email : body?.Email;
        var p = !string.IsNullOrWhiteSpace(password) ? password : body?.Password;

        if (string.IsNullOrWhiteSpace(e) || string.IsNullOrWhiteSpace(p))
            return BadRequest(new { message = "Email and password are required" });

        var result = _us.Login(e, p);
        if (result == null) return Unauthorized(new { message = "Invalid email/password or account not verified" });

        return Ok(result);
    }

    [HttpPost("verify")]
    public IActionResult VerifyUser([FromQuery] string? email, [FromQuery] string? code, [FromBody] VerifyRequest? body = null)
    {
        var e = !string.IsNullOrWhiteSpace(email) ? email : body?.Email;
        var c = !string.IsNullOrWhiteSpace(code) ? code : body?.Code;

        if (string.IsNullOrWhiteSpace(e) || string.IsNullOrWhiteSpace(c))
            return BadRequest(new { message = "Email and code are required" });

        var result = _us.Verify(e, c);
        if (result == null) return BadRequest(new { message = "Invalid code or user already verified" });

        return Ok(result);
    }
}
