using Dannys.Models.Common;

namespace Dannys.Models
{
    public class Order:BaseAuditableEntity
	{
		public bool? Status { get; set; }
		public bool IsCanceled { get; set; }

        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string StreetAdrees { get; set; } = null!;
        public string? Apartment { get; set; } 

        public AppUser AppUser { get; set; } = null!;
		public string AppUserId { get; set; } = null!;

        public Coupon? Coupon { get; set; }
        public int? CouponId { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}

