using System;
using System.ComponentModel.DataAnnotations;

namespace Dannys.Dtos;

public class LoginDto
{
    public string EmailOrUsername { get; set; } = null!;
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}

