using Dannys.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Dannys.Data;

namespace Dannys.Controllers;

public class ShopController : Controller
{

    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public ShopController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var query =await _context.Products.Include(x => x.ProductImgs).ToListAsync();
        
        return View(query);
    }


    public async Task<IActionResult> AddToBasket(int id, string? returnUrl)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return NotFound();


        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return BadRequest();

            var dbBasketItems = await _context.Basketitems.Where(x => x.AppUserId == userId).ToListAsync();


            var existBItem = dbBasketItems.FirstOrDefault(x => x.ProductId == id);
            if (existBItem is not null)
            {
                existBItem.Count++;
                _context.Basketitems.Update(existBItem);
            }
            else
            {
                Basketitem bItem = new() { AppUserId = userId, ProductId = id, Count = 1 };
                await _context.Basketitems.AddAsync(bItem);
            }

            await _context.SaveChangesAsync();
        }
        else
        {


            List<Basketitem> basketItems = GetBasket();

            var existItem = basketItems.FirstOrDefault(x => x.ProductId == id);

            if (existItem is not null)
                existItem.Count++;
            else
            {
                Basketitem vm = new() { ProductId = id, Count = 1 };
                basketItems.Add(vm);
            }

            var json = JsonConvert.SerializeObject(basketItems);
            Response.Cookies.Append("basket", json);

        }


        if (returnUrl is not null)
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));

    }


    public async Task<IActionResult> Detail(int id)
    {
        var existProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (existProduct is null) return BadRequest();


        var product = await _context.Products.Include(x => x.ProductImgs)
                                             .FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) return NotFound();
        return View(product);
    }

        private List<Basketitem> GetBasket()
    {
        List<Basketitem> basketItems = new();
        if (Request.Cookies["basket"] != null)
        {
            basketItems = JsonConvert.DeserializeObject<List<Basketitem>>(Request.Cookies["basket"]) ?? new();
        }

        return basketItems;
    }
}

