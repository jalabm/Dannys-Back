namespace Dannys.Dtos;

public class OrderCreateDto
{
    
    public string PhoneNumber { get; set; } = null!;
    public string City { get; set; } = null!;
    public string StreetAdrees { get; set; } = null!;
    public string? Apartment { get; set; }
    public string stripeToken { get; set; } = null!;
    public string stripeEmail { get; set; } = null!;
}

