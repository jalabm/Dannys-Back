using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Order:BaseAuditableEntity
	{
		public string AppUserId { get; set; } = null!;
		public bool Status { get; set; }
		public bool IsCanceled { get; set; }

		public AppUser AppUser { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}

