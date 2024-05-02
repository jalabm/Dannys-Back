using System;
namespace Dannys.Dtos
{
    public class AuthorCreateDto
	{
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Biographia { get; set; } = null!;
    }
}

