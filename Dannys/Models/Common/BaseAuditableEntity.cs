using System;
namespace Dannys.Models.Common
{
	public class BaseAuditableEntity:BaseEntity
	{
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public string CreatedBy { get; set; } = null!;
		public string? UpdatedBy { get; set; }
		public bool IsDeleted { get; set; }
	}
}

