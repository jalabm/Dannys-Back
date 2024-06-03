using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Table:BaseEntity
	{
		public int TableNo { get; set; }
		public int PersonCount { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    }
}

