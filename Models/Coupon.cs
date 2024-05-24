using System.ComponentModel.DataAnnotations;
using Dannys.Models.Common;

namespace Dannys.Models;

public class Coupon:BaseAuditableEntity
{
	public string Name { get; set; } = null!;
	[Range(0,100)]
    [DisplayFormat(DataFormatString = "{0:0.0}", ApplyFormatInEditMode = true)] 
    public decimal Discount { get; set; }
}

