using Dannys.Models.Common;

namespace Dannys.Models;

    public class Category:BaseAuditableEntity
{
	public string Title { get; set; } = null!;
        public string SubTitle { get; set; } = null!;
        public int  Order { get; set; }

	public ICollection<Product> Products { get; set; } = new List<Product>();
}

