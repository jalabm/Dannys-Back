using System;
using AutoMapper;

namespace Dannys.AutoMappers
{
	public class BlogAutoMapper:Profile
	{
		public BlogAutoMapper()
		{
            CreateMap<Blog, BlogCreatedto>().ReverseMap();
            CreateMap<Blog, BlogUpdatedto>().ReverseMap();
        }
	}
}

