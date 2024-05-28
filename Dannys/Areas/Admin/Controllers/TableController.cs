using Dannys.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
public class TableController : Controller
{
    private readonly AppDbContext _context;
    

    public TableController(AppDbContext context)
    {
        _context = context;
    }



    public async Task<IActionResult> Index()
    {
        var tables = await _context.Tables.ToListAsync();
        return View(tables);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(TableCreateDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);
        Table table = new() { PersonCount = dto.PersonCount };

        await _context.Tables.AddAsync(table);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}

