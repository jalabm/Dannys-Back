using System;
using System.ComponentModel.DataAnnotations;

namespace Dannys.Dtos;

public class CommentCreateDto
{
	public int ProductId{ get; set; }
	public string Text { get; set; } = null!;
	[Range(0,5)]
	public int Rating { get; set; }
}

