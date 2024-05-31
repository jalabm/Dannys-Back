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

    public async Task<IActionResult> Index(int? topicId)
    {
        var topics = await _context.Topics.ToListAsync();
        var query = _context.Blogs.Include(x => x.Author).Include(x => x.BlogTopics).ThenInclude(x => x.Topic);
        List<Blog> blogs = new();
        if (topicId is not null)
        {
            blogs = await query.Where(x => x.BlogTopics.Any(y => y.TopicId == topicId)).ToListAsync();

        }
        else
        {
            blogs = await query.ToListAsync();
        }

        BlogVM blogVm = new BlogVM()
        {
            Blogs = blogs,
            Topics = topics

        };
        return View(blogVm);
    }


    public async Task<IActionResult> Detail(int id)
    {
        var topics = await _context.Topics.ToListAsync();
        var blog = await _context.Blogs.Include(x => x.Author).Include(x => x.BlogTopics).ThenInclude(x => x.Topic).FirstOrDefaultAsync(x => x.Id == id);


        if (blog is null)
            return NotFound();

        BlogDetailVM blogVm = new()
        {
            Blog = blog,
            Topics = topics

        };

        blogVm.NextBlog = await _context.Blogs.FirstOrDefaultAsync(x => x.Id > id);
        blogVm.PrevBlog = await _context.Blogs.OrderByDescending(x=>x.Id).FirstOrDefaultAsync(x => x.Id < id);

        if(blogVm.NextBlog is null)
            blogVm.NextBlog= await _context.Blogs.FirstOrDefaultAsync(x => x.Id < id);

        if(blogVm.PrevBlog is null)
            blogVm.PrevBlog= await _context.Blogs.OrderByDescending(x=>x.Id).FirstOrDefaultAsync(x => x.Id > id);

        return View(blogVm);
    }
}

