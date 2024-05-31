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
    public async Task<IActionResult> Index(int page=1)
    {
        int pageCount = (int)Math.Ceiling((decimal)_context.Topics.Count() / 4);

        if (pageCount == 0)
            pageCount = 1;

        ViewBag.PageCount = pageCount;
        ViewBag.Page = page;

       

        var query =await _context.Products.Take(page*4).Include(x => x.ProductImgs).ToListAsync();
        
        return View(query);
    }

    public async Task<IActionResult> LoadMore(int page)
    {
        int pageCount = (int)Math.Ceiling((decimal)_context.Products.Count() / 4);

        if (pageCount == 0)
            pageCount = 1;


        if (page > pageCount)
            page = pageCount;

        if (page <= 0)
            page = 1;



        var products = await _context.Products.Skip((page-1)*4).Take(4).Include(x => x.ProductImgs).ToListAsync();


        foreach (var product in products)
        {
            foreach (var img in product.ProductImgs)
            {
                img.Product = null;
            }

        }



        return Ok(products);
    }


    public async Task<IActionResult> AddToBasket(int id, string? returnUrl,int count=1,int page=1)
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
                existBItem.Count+=count;
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
                existItem.Count+=count;
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

