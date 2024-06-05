using System;
using System.ComponentModel.DataAnnotations;

namespace Dannys.ViewModels
{
	public class ContactVM
	{
        public string Fullname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}

