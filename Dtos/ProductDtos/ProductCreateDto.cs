using System.ComponentModel.DataAnnotations;
using Dannys.Models;

namespace Dannys.Dtos;

public class ProductCreateDto
{

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Ingredients { get; set; } = null!;
    public string Porsion { get; set; } = null!;
    [Range(0,100)]
    public decimal Discount { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int SalesCount { get; set; }

    public IFormFile MainFile { get; set; } = null!;
    public List<IFormFile> AdditionalFiles { get; set; } = new();

}





