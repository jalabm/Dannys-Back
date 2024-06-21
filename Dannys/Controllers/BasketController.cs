using System.Security.Claims;
using Dannys.Data;
using Dannys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stripe;


namespace Dannys.Controllers;

public class BasketController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public BasketController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var basketItems = await GetBasketAsync();


        return View(basketItems);

    }

    public async Task<IActionResult> RemoveToBasket(int id, string? returnUrl)
    {
        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var basketItem = await _context.Basketitems.FirstOrDefaultAsync(x => x.ProductId == id && x.AppUserId == userId);
            if (basketItem is null)
            {
                return NotFound();
            }

            _context.Basketitems.Remove(basketItem);
            await _context.SaveChangesAsync();

            if (returnUrl is not null)
                return Redirect(returnUrl);

            return RedirectToAction("index");
        }
        var basketItms = _getBasket();

        var basketItm = basketItms.FirstOrDefault(x => x.ProductId == id);
        if (basketItm is null)
            return NotFound();

        basketItms.Remove(basketItm);
        var json = JsonConvert.SerializeObject(basketItms);
        Response.Cookies.Append("basket", json);

        if (returnUrl is not null)
            return Redirect(returnUrl);

        return RedirectToAction("index");
    }



    public async Task<IActionResult> Checkout()
    {
        var basketItems = await GetBasketAsync();
        return View(basketItems);
    }

    [HttpPost]
    [Authorize]

    public async Task<IActionResult> Checkout(OrderCreateDto dto)
    {


        if (!ModelState.IsValid)
            return RedirectToAction("Checkout");


        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();


        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return BadRequest();

        var basketItems = await GetBasketAsync();

        decimal total = 0;

        basketItems.ForEach(bi =>
        {
            total += bi.Count * bi.Product.Price;
        });

        var optionCust = new CustomerCreateOptions
        {
            Email = dto.stripeEmail,
            Name = user.Fullname,
            Phone = "000000"
        };
        var serviceCust = new CustomerService();
        Customer customer = serviceCust.Create(optionCust);

        total = total * 100;
        var optionsCharge = new ChargeCreateOptions
        {

            Amount = (long)total,
            Currency = "USD",
            Description = "Dannys order",
            Source = dto.stripeToken,
            ReceiptEmail = "jalabm@code.edu.az"


        };
        var serviceCharge = new ChargeService();
        Charge charge = serviceCharge.Create(optionsCharge);


        Order order = new()
        {
            AppUser = user,
            Status = false,
            OrderItems = new List<OrderItem>(),
            PhoneNumber = dto.PhoneNumber,
            City = dto.City,
            Apartment = dto.Apartment,
            StreetAdrees=dto.StreetAdrees
        };

        foreach (var bItem in basketItems)
        {
            OrderItem orderItem = new()
            {
                Order = order,
                Product = bItem.Product,
                Count = bItem.Count,
                StaticPrice = bItem.Product.Price,


            };
            order.OrderItems.Add(orderItem);

            _context.Basketitems.Remove(bItem);
        }
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();


        return RedirectToAction("Index", "Home");

    }


    public async Task<IActionResult> EditBasketItem(int id, int count)
    {
        if (count < 1)
            return RedirectToAction("Index");


        var basketItems = await GetBasketAsync();


        var basketItem = basketItems.FirstOrDefault(x => x.Id == id);

        if (basketItem is null)
            return NotFound();

        basketItem.Count = count;


        if (User.Identity.IsAuthenticated)
        {
            _context.Basketitems.Update(basketItem);
            await _context.SaveChangesAsync();
        }
        else
        {
            var json = JsonConvert.SerializeObject(basketItems);
            Response.Cookies.Append("basket", json);
        }

        return RedirectToAction("Index");

    }

    private List<Basketitem> _getBasket()
    {
        List<Basketitem> basketItems = new();
        if (Request.Cookies["basket"] != null)
        {
            basketItems = JsonConvert.DeserializeObject<List<Basketitem>>(Request.Cookies["basket"]) ?? new();
        }

        return basketItems;
    }


    private async Task<List<Basketitem>> GetBasketAsync()
    {
        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var basketItems = await _context.Basketitems.Include(x => x.Product).ThenInclude(x => x.ProductImgs).Where(x => x.AppUserId == userId).ToListAsync();
            return basketItems;

        }

        var basktItms = _getBasket();
        foreach (var item in basktItms)
        {
            var product = await _context.Products.Include(x => x.ProductImgs).FirstOrDefaultAsync(x => x.Id == item.ProductId);
            item.Product = product;


        }

        return basktItms;
    }
}

