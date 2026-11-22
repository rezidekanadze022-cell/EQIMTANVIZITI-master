using AutoMapper;
using EQIMTANVIZITI.Data;
using EQIMTANVIZITI.DependencyInjection.SMTP;
using EQIMTANVIZITI.DTOs;
using EQIMTANVIZITI.Enums;
using EQIMTANVIZITI.Models;
using Microsoft.EntityFrameworkCore;

namespace EQIMTANVIZITI.DependencyInjection.Users;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var userexists = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (userexists != null) return false;

        string hashedpassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var mapped = _mapper.Map<User>(request);
        mapped.Email = email;
        mapped.Password = hashedpassword;
        mapped.Role = Role.Patient;

        if (string.IsNullOrWhiteSpace(mapped.FirstName) && !string.IsNullOrWhiteSpace(request.FullName))
        {
            var parts = request.FullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            mapped.FirstName = parts[0];
            mapped.LastName = parts.Length > 1 ? parts[1] : parts[0];
        }

        await SMTPService.SendEmail(mapped);

        _context.Users.Add(mapped);
        await _context.SaveChangesAsync();

        return true;
    }

    public User? Login(string email, string password)
    {
        var e = email.Trim().ToLowerInvariant();
        var userexists = _context.Users.FirstOrDefault(u => u.Email == e && u.IsVerified == true);

        if (userexists == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, userexists.Password)) return null;

        return userexists;
    }

    public User? Verify(string email, string code)
    {
        var e = email.Trim().ToLowerInvariant();
        var userexists = _context.Users.FirstOrDefault(u => u.Email == e && u.Code == code);

        if (userexists == null) return null;
        if (userexists.IsVerified == true) return null;

        userexists.Code = null;
        userexists.CodeExpiresAt = null;
        userexists.IsVerified = true;

        _context.SaveChanges();
        return userexists;
    }
}
