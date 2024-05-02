using System;
using Dannys.Models.Common;

namespace Dannys.Models
{
	public class Slider:BaseAuditableEntity
	{
		public string Url { get; set; } = null!;
		public string Name { get; set; } = null!;
	}
}

