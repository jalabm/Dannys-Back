using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Author:BaseAuditableEntity
	{
		public string Name { get; set; } = null!;
		public string Surname { get; set; } = null!;
		public string Description { get; set; } = null!;
		public string Biographia { get; set; } = null!;
        [NotMapped]
        public string Fullname { get => $"{Name} {Surname} "; }
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    }
}

