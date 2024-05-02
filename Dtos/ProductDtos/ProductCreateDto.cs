using Dannys.Models;

namespace Dannys.Dtos;

public class ProductCreateDto
{

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Ingredients { get; set; } = null!;
    public string Porsion { get; set; } = null!;
    public decimal Discount { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int SalesCount { get; set; }
}


public class ProductUpdateDto
{

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Ingredients { get; set; } = null!;
    public string Porsion { get; set; } = null!;
    public decimal Discount { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int SalesCount { get; set; }
}



public class ProductGetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Ingredients { get; set; } = null!;
    public string Porsion { get; set; } = null!;
    public decimal Discount { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int SalesCount { get; set; }
}

