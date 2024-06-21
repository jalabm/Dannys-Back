using System.Diagnostics;
using System.Security.Claims;
using Dannys.Data;
using Dannys.Models;
using Dannys.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Dannys.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    private readonly UserManager<AppUser> _userManager;


    public HomeController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        //ViewBag.Page = page;
        var products = await _context.Products.Include(x => x.ProductImgs).Include(x => x.Category).Take(12).ToListAsync();
        var categories = await _context.Categories.Where(x=>x.Products.Count>0).Take(10).ToListAsync();
        var sliders = await _context.Sliders.ToListAsync();
        var topComments = await _context.Comments.OrderByDescending(x => x.Rating).Include(x => x.AppUser).Take(3).ToListAsync();


        HomeVM homeVM = new HomeVM()
        {
            Products = products,
            Categories = categories,
            Sliders = sliders,
            Comments=topComments

        };
        return View(homeVM);
    }

    public async Task<IActionResult> AddToBasket(int id, string? returnUrl, int count = 1, int page = 1)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);


        if (product is null)
            return NotFound();
        if (count < 1)
            count = 1;
        if (page < 1)
            page = 1;

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
                existBItem.Count += count;
                _context.Basketitems.Update(existBItem);
            }
            else
            {
                Basketitem bItem = new() { AppUserId = userId, ProductId = id, Count = count };


                await _context.Basketitems.AddAsync(bItem);
            }

            await _context.SaveChangesAsync();
        }
        else
        {


            List<Basketitem> basketItems = GetBasket();

            var existItem = basketItems.FirstOrDefault(x => x.ProductId == id);

            if (existItem is not null)
                existItem.Count += count;
            else
            {
                Basketitem vm = new() { ProductId = id, Count = count };
                basketItems.Add(vm);
            }

            var json = JsonConvert.SerializeObject(basketItems);
            Response.Cookies.Append("basket", json);

        }


        if (returnUrl is not null)
            return Redirect(returnUrl);

        return RedirectToAction("Index", new { page = page });


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


    public async Task<IActionResult> Subscribe(string email, string returnUrl)
    {
        if (!ModelState.IsValid)
            return Redirect(returnUrl);


        var isExist = await _context.Subscribes.AnyAsync(x => x.Email.ToLower() == email.ToLower());

        if (isExist)
            return Redirect(returnUrl);


        Subscribe subscribe = new()
        {
            Email = email
        };

        await _context.Subscribes.AddAsync(subscribe);
        await _context.SaveChangesAsync();

        return Redirect(returnUrl);
    }

}

