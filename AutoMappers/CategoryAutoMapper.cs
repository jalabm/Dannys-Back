using AutoMapper;

namespace Dannys.AutoMappers;

public class CategoryAutoMapper:Profile
	{
		public CategoryAutoMapper()
		{
        CreateMap<Category, CategoryCreateDto>().ReverseMap();
        CreateMap<Category, CategoryUpdateDto>().ReverseMap();
    }

	}

