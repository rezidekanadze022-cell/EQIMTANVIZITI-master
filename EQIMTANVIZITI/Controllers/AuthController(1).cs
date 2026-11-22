using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EQIMTANVIZITI.Controllers;

[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")] public async Task<IActionResult> Register(RegisterRequest r) => Ok(await _auth.RegisterAsync(r));
    [HttpPost("verify")] public async Task<IActionResult> Verify(VerifyRequest r) => Ok(await _auth.VerifyAsync(r));
    [HttpPost("resend-code")] public async Task<IActionResult> Resend(ResendCodeRequest r) => Ok(await _auth.ResendCodeAsync(r));
    [HttpPost("login")] public async Task<IActionResult> Login(LoginRequest r) => Ok(await _auth.LoginAsync(r));
}
