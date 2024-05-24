using AutoMapper;

namespace Dannys.AutoMappers;

public class ProductAutoMapper:Profile
{
    public ProductAutoMapper()
    {
        CreateMap<Product, ProductCreateDto>().ReverseMap();
        CreateMap<Product, ProductUpdateDto>().ReverseMap();

    }
}

