using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class OrderItem:BaseAuditableEntity
	{
		public int ProductId { get; set; }
		public int OrderId { get; set; }
		public Product Product { get; set; } = null!;
		public Order Order { get; set; } = null!;
    }
}

