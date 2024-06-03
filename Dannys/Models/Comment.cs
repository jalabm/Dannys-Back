using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Comment:BaseAuditableEntity
	{
		public string AppUserId { get; set; } = null!;
		public AppUser AppUser { get; set; } = null!;
		public decimal Rating { get; set; }
		public string Text { get; set; } = null!;
		//public int? ParentId { get; set; }
		//public Comment? Parent { get; set; }
		public int ProductId { get; set; }
		public Product Product { get; set; }=null!;
	}
}

