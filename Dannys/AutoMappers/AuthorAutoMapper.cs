using System;
using AutoMapper;

namespace Dannys.AutoMappers;

public class AuthorAutoMapper : Profile
{
    public AuthorAutoMapper()
    {
        CreateMap<Author, AuthorCreateDto>().ReverseMap();
        CreateMap<Author, AuthorUpdateDto>().ReverseMap();
    }
}

