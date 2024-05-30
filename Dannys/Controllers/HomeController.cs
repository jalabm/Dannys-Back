using System.Diagnostics;
using Dannys.Data;
using Dannys.Models;
using Dannys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.Include(x => x.ProductImgs).Include(x => x.Category).ToListAsync();
        var categories = await _context.Categories.ToListAsync();
        var sliders =await _context.Sliders.ToListAsync();
        HomeVM homeVM = new HomeVM()
        {
            Products = products,
            Categories = categories,
            Sliders=sliders

        };
        return View(homeVM);
    }
} 

