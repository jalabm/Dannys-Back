﻿using Dannys.Data;
using Dannys.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrderController : Controller
{
    private readonly AppDbContext _context;

    public OrderController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        int pageCount = (int)Math.Ceiling((decimal)_context.Orders.Count() / 10);

        if (pageCount == 0)
            pageCount = 1;

        ViewBag.PageCount = pageCount;

        if (page > pageCount)
            page = pageCount;

        if (page <= 0)
            page = 1;

        ViewBag.CurrentPage = page;


        var orders = await _context.Orders.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * 10).Take(10).Include(x => x.AppUser).ToListAsync();



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
                AppUserId = "1",

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

    public async Task<IActionResult> Detail(int id)
    {
        var order = await _context.Orders.Include(x => x.OrderItems).ThenInclude(x=>x.Product).ThenInclude(x=>x.ProductImgs).FirstOrDefaultAsync(x => x.Id == id);
        return View(order);
    }
}

