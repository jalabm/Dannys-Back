using System;
using AutoMapper;

namespace Dannys.AutoMappers;

public class AppUserAutoMapper:Profile
{
    public AppUserAutoMapper()
    {
        CreateMap<AppUser, RegisterDto>().ReverseMap();
    }
}

