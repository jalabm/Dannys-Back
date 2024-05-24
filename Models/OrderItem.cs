using Dannys.Models.Common;

namespace Dannys.Models
{
    public class OrderItem:BaseEntity
	{
		public int Count { get; set; }
		public decimal StaticPrice { get; set; }

		public Product Product { get; set; } = null!;
		public int ProductId { get; set; }

		public Order Order { get; set; } = null!;
		public int OrderId { get; set; }

		
	}
}

