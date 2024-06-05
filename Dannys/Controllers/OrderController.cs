using System.Security.Claims;
using Dannys.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Controllers;

public class OrderController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public OrderController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [Authorize]
    public async  Task<IActionResult> Index()
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return BadRequest();

        var orders = await _context.Orders.Where(x=>x.AppUserId==userId).OrderByDescending(x=>x.Id).Include(x => x.OrderItems).ThenInclude(x => x.Product).ThenInclude(x => x.ProductImgs).ToListAsync();
        return View(orders);
    }
}

