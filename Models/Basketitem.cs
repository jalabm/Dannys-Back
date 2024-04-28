using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Basketitem:BaseEntity
	{
		public int ProductId { get; set; }
		public string AppUserId { get; set; } = null!;

		public Product Product { get; set; } = null!;
		public AppUser AppUser { get; set; } = null!;
	}
}

