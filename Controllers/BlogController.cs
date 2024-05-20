using Dannys.Data;
using Dannys.Models;
using Dannys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Controllers;

public class BlogController : Controller
{

    private readonly AppDbContext _context;

    public BlogController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var topics = await _context.Topics.ToListAsync();
        var blog = await _context.Blogs.Include(x => x.Author).Include(x => x.BlogTopics).ThenInclude(x => x.Topic).ToListAsync();
        BlogVM blogVm = new BlogVM()
        {
            Blogs = blog,
            Topics =topics

        };
        return View(blogVm);
    }


    public async Task<IActionResult> Detail(int id)
    {
        var topics = await _context.Topics.ToListAsync();
        var blog = await _context.Blogs.Include(x => x.Author).Include(x => x.BlogTopics).ThenInclude(x => x.Topic).FirstOrDefaultAsync(x=>x.Id==id);
        BlogVM blogVm = new BlogVM()
        {
            Blog = blog,
            Topics = topics

        };
        return View(blogVm);
    }
}

