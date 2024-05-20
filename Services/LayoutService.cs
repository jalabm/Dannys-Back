using System;
using Dannys.Data;
using Dannys.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Services
{
	public class LayoutService
	{
        private readonly IHttpContextAccessor _httpContext;
        private readonly AppDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public LayoutService(IHttpContextAccessor httpContext, AppDbContext context, UserManager<AppUser> userManager)
        {
            _httpContext = httpContext;
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Basketitem>> GetBasket()
        {
            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                {
                    return new();
                }
                var basketItems = await _context.Basketitems.Include(x => x.Product).ThenInclude(x => x.ProductImgs).Where(x => x.AppUserId == userId).ToListAsync();
                return basketItems;

            }

            var basktItms = _getBasket();
            foreach (var item in basktItms)
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                item.Product = product;

            }
            return basktItms;


        }
        //public async Task<Dictionary<string, string>> GetSettingsAsync()
        //{
        //    var settings = await _context.Settings.ToDictionaryAsync(x => x.Key, x => x.Value);

        //    return settings;
        //}

        public async Task<List<Category>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            return categories;
        }

        private List<Basketitem> _getBasket()
        {
            List<Basketitem> basketItems = new();
            if (_httpContext.HttpContext.Request.Cookies["basket"] != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<Basketitem>>(_httpContext.HttpContext.Request.Cookies["basket"]) ?? new();
            }

            return basketItems;
        }
    }
}

