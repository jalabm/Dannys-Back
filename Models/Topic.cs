using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Topic:BaseEntity
	{
		public string Name { get; set; } = null!;
        public ICollection<BlogTopic> BlogTopics { get; set; } = new List<BlogTopic>();

    }
}

