using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Topic:BaseAuditableEntity
	{
		public string Name { get; set; } = null!;
        public ICollection<BlogTopic> BlogTopics { get; set; } = new List<BlogTopic>();

    }
}

