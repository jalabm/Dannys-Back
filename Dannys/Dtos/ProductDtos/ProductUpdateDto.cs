﻿using System.ComponentModel.DataAnnotations;

namespace Dannys.Dtos;

public class ProductUpdateDto
{

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Ingredients { get; set; } = null!;
    public int Porsion { get; set; } 
    [Range(0, 100)]
    public decimal Discount { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int SalesCount { get; set; }

    public string? MainFileUrl { get; set; } 
    public IFormFile? MainFile { get; set; } = null!;
    public List<IFormFile> AdditionalFiles { get; set; } = new();
    public List<string> ImagePaths { get; set; } = new();
    public List<int> ImageIds { get; set; } = new();
}




