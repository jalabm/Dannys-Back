using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Author:BaseAuditableEntity
	{
		public string FullName { get; set; } = null!;
		public string Description { get; set; } = null!;
		public string Biographia { get; set; } = null!;
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    }
}

