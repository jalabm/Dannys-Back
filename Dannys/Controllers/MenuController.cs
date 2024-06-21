using Dannys.Data;
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

    public async  Task<IActionResult> Index(int? categoryId)
    {
        if(categoryId is not null)
        {
            var category = await _context.Categories.Include(x => x.Products).ThenInclude(x => x.ProductImgs).FirstOrDefaultAsync(x => x.Id == categoryId);

            if (category is null || category.Products.Count() == 0)
                return NotFound();

            return View(new List<Category>() { category });

        }
        var categories = await _context.Categories.Include(x => x.Products).ThenInclude(x=>x.ProductImgs).Where(x => x.Products.Count > 0).ToListAsync();
        return View(categories);
    }
}

