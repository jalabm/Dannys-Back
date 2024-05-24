using System;
using System.ComponentModel.DataAnnotations;

namespace Dannys.Dtos;

public class CouponCreateDto
{
	public string Name { get; set; } = null!;
    [Range(0, 100)]
    [DisplayFormat(DataFormatString = "{0:0.0}", ApplyFormatInEditMode = true)]
    public decimal Discount { get; set; }

}

