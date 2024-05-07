using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dannys.Data;
using Dannys.Enums;
using Dannys.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Dannys.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _context;

    public AccountController(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
    {
        _userManager = userManager;
        _emailService = emailService;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
    }

    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        AppUser appUser = new()
        {
            UserName = dto.Username,
            Email = dto.Email,
            Surname = dto.Surname,
            Name = dto.Name
        };
        var result = await _userManager.CreateAsync(appUser, dto.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);


            }
            return View();
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

        string? url = Url.Action("ConfirmEmail", "Account", new { userId = appUser.Id, token = token }, HttpContext.Request.Scheme);


        _emailService.SendEmail(appUser.Email, "Confirm Email", url);

        await _userManager.AddToRoleAsync(appUser, Roles.Member.ToString());

        await TransferBasket(appUser);
        return RedirectToAction("Login");
    }

    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();


        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return BadRequest();


        await _signInManager.SignInAsync(user, false);

        await TransferBasket(user);

        return RedirectToAction("Index", "Home");


    }

    public IActionResult Login()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid) return View(   dto);

        var existUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existUser == null)
        {
            ModelState.AddModelError("", "Invalid Credentials");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(existUser, dto.Password, dto.RememberMe, true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid Credentials");
            return View();
        }

        await TransferBasket(existUser);

        var role = await _userManager.IsInRoleAsync(existUser, Roles.Admin.ToString());
        if (role)
            return RedirectToAction("Index", "Dashboard", new { Area = "Admin" });
        return RedirectToAction("Index", "Home");

    }

    private async Task TransferBasket(AppUser existUser)
    {
        var basketItems = _getBasket();

        foreach (var item in basketItems)
        {
            var isExistBasket = await _context.Basketitems.AnyAsync(x => x.ProductId == item.ProductId && x.AppUserId == existUser.Id);

            if (!isExistBasket)
            {
                Basketitem basketItem = new() { ProductId = item.ProductId, AppUserId = existUser.Id, Count = item.Count };
                await _context.Basketitems.AddAsync(basketItem);
            }

        }
        await _context.SaveChangesAsync();
    }

    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> CreateRole()
    {
        foreach (var role in Enum.GetValues(typeof(Roles)))
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = role.ToString(),
            });
        }
        return RedirectToAction("Index", "Home");
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
}

