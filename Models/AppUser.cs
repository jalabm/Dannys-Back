using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Dannys.Models
{
	public class AppUser:IdentityUser
	{
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        [NotMapped]
        public string Fullname { get => $"{Name} {Surname} "; }
        public ICollection<Basketitem> Basketitems { get; set; } = new List<Basketitem>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();


    }
}

