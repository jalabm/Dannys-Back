using System;
namespace Dannys.Dtos
{
    public class SliderCreateDto
	{
        public string Name { get; set; } = null!;
        public IFormFile File { get; set; } = null!;
    }
}

