using System;
using AutoMapper;

namespace Dannys.AutoMappers;

public class TopicAutoMapper:Profile
{
	public TopicAutoMapper()
	{
		CreateMap<Topic, TopicCreateDto>().ReverseMap();
		CreateMap<Topic, TopicUpdateDto>().ReverseMap();
	}
}

