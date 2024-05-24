using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class ProductImg:BaseEntity
	{
        public string Url { get; set; } = null!;
        public bool IsMain { get; set; }
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}

