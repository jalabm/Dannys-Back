namespace Dannys.Dtos;

public class BlogUpdatedto
{
    public string Title { get; set; } = null!;
    public string ShortDescription { get; set; } = null!;
    public string LongDescription { get; set; } = null!;
    public int AuthorId { get; set; }
    public List<int> TopicIds { get; set; } = new();

}

