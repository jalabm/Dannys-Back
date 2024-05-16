using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Blog:BaseAuditableEntity
	{
		public string Title { get; set; } = null!;
		public string ShortDescription { get; set; } = null!;
		public string LongDescription { get; set; } = null!;
		public int AuthorId { get; set; }
		public string ImageUrl { get; set; } = null!;
		public Author Author { get; set; } = null!;

        public ICollection<BlogTopic> BlogTopics { get; set; } = new List<BlogTopic>();

    }
}

