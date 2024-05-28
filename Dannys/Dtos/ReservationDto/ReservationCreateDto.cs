using System.ComponentModel.DataAnnotations;

namespace Dannys.Dtos;

public class ReservationCreateDto
{
    [Required(ErrorMessage = "TableId is required")]
    public int TableId { get; set; }

    [Required(ErrorMessage = "Time is required")]
    public string Time { get; set; } = null!;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "PhoneNumber is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; } = null!;

}

