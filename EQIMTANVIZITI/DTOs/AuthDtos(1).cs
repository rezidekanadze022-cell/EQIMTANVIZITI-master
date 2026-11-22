namespace EQIMTANVIZITI.DTOs;

public class RegisterRequest
{
    public string? FullName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Age { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;

    public RegisterRequest() { }

    public RegisterRequest(string firstName, string lastName, string email, string phoneNumber, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        FullName = $"{firstName} {lastName}".Trim();
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
    }
}

public record VerifyRequest(string Email, string Code)
{
    public VerifyRequest() : this(string.Empty, string.Empty) { }
}

public record LoginRequest(string Email, string Password)
{
    public LoginRequest() : this(string.Empty, string.Empty) { }
}

public record ResendCodeRequest(string Email)
{
    public ResendCodeRequest() : this(string.Empty) { }
}

public record MessageResponse(string Message);

public class AuthResponse
{
    public string Token { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
}
