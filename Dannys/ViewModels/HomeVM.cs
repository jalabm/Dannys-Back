using System;
namespace Dannys.ViewModels
{
	public class HomeVM
	{
		public List<Product> Products { get; set; } = new();
		public List<Category> Categories { get; set; } = new();
		public List<Slider> Sliders { get; set; } = new();
		public List<Comment> Comments { get; set; } = new();

	}
}
