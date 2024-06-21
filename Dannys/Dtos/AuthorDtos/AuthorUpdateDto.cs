namespace Dannys.Dtos
{
    public class AuthorUpdateDto
	{
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Biographia { get; set; } = null!;
        public IFormFile? Image { get; set; } 

    }
}

