﻿using Dannys.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Controllers;

public class MenuController : Controller
{
    private readonly AppDbContext _context;

    public MenuController(AppDbContext context)
    {
        _context = context;
    }

    public async  Task<IActionResult> Index()
    {

        var category = await _context.Categories.Include(x => x.Products).ToListAsync();
        return View(category);
    }
}
