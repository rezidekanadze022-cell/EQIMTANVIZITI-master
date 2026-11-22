namespace EQIMTANVIZITI.DTOs;

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Age { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}
public record VerifyRequest(string Email, string Code);
public record LoginRequest(string Email, string Password);
public record ResendCodeRequest(string Email);
public record MessageResponse(string Message);

public class AuthResponse
{
    public string Token { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
}