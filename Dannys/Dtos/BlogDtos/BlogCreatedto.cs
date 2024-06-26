﻿namespace Dannys.Dtos;

public class BlogCreatedto
{
    public string Title { get; set; } = null!;
    public string ShortDescription { get; set; } = null!;
    public string LongDescription { get; set; } = null!;
    public int AuthorId { get; set; }
    public IFormFile Image { get; set; } = null!;
    public List<int> TopicIds { get; set; } = new();
}

