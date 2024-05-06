using System;
using AutoMapper;

namespace Dannys.AutoMappers
{
	public class SliderAutoMapper:Profile
	{
		public SliderAutoMapper()
		{
            CreateMap<Slider, SliderCreateDto>().ReverseMap();
            CreateMap<Slider, SliderUpdateDto>().ReverseMap();
        }
	}
}

