using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Reservation:BaseAuditableEntity
	{
		public int TableId { get; set; }
		public string AppUserId { get; set; } = null!;
		public Table Table { get; set; } = null!;
		public AppUser AppUser { get; set; } = null!;
		public DateTime Date { get; set; }
	}
}

