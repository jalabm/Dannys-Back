using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class BlogTopic:BaseAuditableEntity
	{
		public int BlogId { get; set; }
		public Blog Blog { get; set; } = null!;
        public int TopicId { get; set; }
		public Topic Topic { get; set; } = null!;
	}
}

