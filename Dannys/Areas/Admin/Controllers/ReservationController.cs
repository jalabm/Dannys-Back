using Dannys.Data;
using Dannys.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ReservationController : Controller
{

    private readonly AppDbContext _context;

    public ReservationController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page=1)
    {


        int pageCount = (int)Math.Ceiling((decimal)_context.Reservations.Count() / 10);

        if (pageCount == 0)
            pageCount = 1;

        ViewBag.PageCount = pageCount;

        if (page > pageCount)
            page = pageCount;

        if (page <= 0)
            page = 1;
        ViewBag.CurrentPage = page;

        var reservations = await _context.Reservations.Include(x=>x.Table).OrderByDescending(x => x.Id).Skip((page - 1) * 10).Take(10).ToListAsync();
        return View(reservations);
    }


    public async Task<IActionResult> RepairOrEnd(int id)
    {
        var reservation = await _context.Reservations.FirstOrDefaultAsync(x => x.Id == id);

        if (reservation is null)
            return NotFound();

        if (reservation.IsDone)
            reservation.IsDone = false;
        else
            reservation.IsDone = true;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");


    }
}

