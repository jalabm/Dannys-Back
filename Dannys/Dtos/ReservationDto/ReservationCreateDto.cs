using System.ComponentModel.DataAnnotations;

namespace Dannys.Dtos;

public class ReservationCreateDto
{
    [Required(ErrorMessage = "TableNo is required")]
    public int TableId { get; set; }

    [Required(ErrorMessage = "Time is required")]
    public DateTime Time { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email adress")]
    public string Email { get; set; } = null!;

}

