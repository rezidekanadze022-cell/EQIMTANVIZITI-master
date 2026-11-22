using System.Security.Claims;
using EQIMTANVIZITI.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EQIMTANVIZITI.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _s;
    public JwtService(IOptions<JwtSettings> options) => _s = options.Value;

    public AuthResponse CreateToken(User user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_s.ExpiresMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_s.Key));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("uid", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("role", user.Role.ToString())
            }),
            Expires = expires,
            Issuer = _s.Issuer,
            Audience = _s.Audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return new AuthResponse
        {
            Token = new JsonWebTokenHandler().CreateToken(descriptor),
            ExpiresAt = expires,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }
}
