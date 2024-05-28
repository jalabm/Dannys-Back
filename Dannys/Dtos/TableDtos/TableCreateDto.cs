using System;
using System.ComponentModel.DataAnnotations;

namespace Dannys.Dtos;

public class TableCreateDto
{
	[Range(0,10)]
	public int PersonCount { get; set; }
}

