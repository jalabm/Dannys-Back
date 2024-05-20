using System;
namespace Dannys.ViewModels
{
	public class BlogVM
	{
        public Blog Blog { get; set; } = new();
        public List<Blog> Blogs { get; set; } = new();
        public List<Topic> Topics { get; set; } = new();
    }
}

