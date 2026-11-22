using AutoMapper;
using EQIMTANVIZITI.DTOs;
using EQIMTANVIZITI.Models;

namespace EQIMTANVIZITI.Helper;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, RegisterRequest>().ReverseMap();
    }
}
