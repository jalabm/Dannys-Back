using Dannys.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
public class OrderController : Controller
{
    private readonly AppDbContext _context;

    public OrderController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {

        var orders = await _context.Orders.OrderByDescending(x => x.Status).Include(x=>x.AppUser).ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Next(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);

        if (order is null)
            return NotFound();

        if (order.Status == false)
            order.Status = null;
        else
            order.Status = true;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
            
    }

    public async Task<IActionResult> TestData()
    {
        for (int i = 0; i < 100; i++)
        {
            Order order = new()
            {
                 AppUserId= "1",
                  
            };


            await _context.Orders.AddAsync(order);
        }

        await _context.SaveChangesAsync();


        return Ok();
    }
    public async Task<IActionResult> Prev(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);

        if (order is null)
            return NotFound();

        if (!order.IsCanceled)
        {
            if (order.Status is true)
                order.Status = null;
            else
                order.Status = false;


            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

        }

        return RedirectToAction("Index");

    }

    public async Task<IActionResult> CancelOrRepair(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);

        if (order is null)
            return NotFound();

        order.IsCanceled = !order.IsCanceled;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");


    }
}

