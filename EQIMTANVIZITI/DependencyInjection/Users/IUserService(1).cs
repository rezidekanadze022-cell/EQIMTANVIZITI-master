using EQIMTANVIZITI.DTOs;
using EQIMTANVIZITI.Models;

namespace EQIMTANVIZITI.DependencyInjection.Users;

public interface IUserService
{
    Task<bool> Register(RegisterRequest request);
    User? Verify(string email, string code);
    User? Login(string email, string password);
}
