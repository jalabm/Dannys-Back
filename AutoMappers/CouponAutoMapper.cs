using System;
using AutoMapper;

namespace Dannys.AutoMappers
{
	public class CouponAutoMapper:Profile
	{
		public CouponAutoMapper()
		{
            CreateMap<Coupon, CouponCreateDto>().ReverseMap();
            CreateMap<Coupon, CouponUpdateDto>().ReverseMap();
        }
	}
}

