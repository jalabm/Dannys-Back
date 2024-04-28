using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Table:BaseAuditableEntity
	{
		public int Persons { get; set; }
		public int IsReservated { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    }
}

