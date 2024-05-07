using System;
using System.ComponentModel.DataAnnotations;

namespace Dannys.Dtos;

public class LoginDto
	{
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}

