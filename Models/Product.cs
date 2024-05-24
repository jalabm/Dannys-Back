using Dannys.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Models
{
    public class Product:BaseAuditableEntity
	{
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
        public string Ingredients { get; set; } = null!;
        public int Porsion { get; set; }
        public decimal Discount { get; set; }
        
        public decimal Price { get; set; }
		public int CategoryId { get; set; }
		public Category Category { get; set; } = null!;
        public int SalesCount { get; set; }

		public ICollection<ProductImg> ProductImgs { get; set; } = new List<ProductImg>();
		public ICollection<Basketitem> Basketitems { get; set; } = new List<Basketitem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();



    }
}

