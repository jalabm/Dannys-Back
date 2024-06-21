using Dannys.Data;
using Dannys.Enums;
using Dannys.Extensions;
using Dannys.Models;
using Dannys.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BlogController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly CloudinaryService _cloudinaryService;

    public BlogController(AppDbContext context, IMapper mapper, CloudinaryService cloudinaryService)
    {
        _context = context;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<IActionResult> Index(int page=1)
    {

        int pageCount = (int)Math.Ceiling((decimal)_context.Blogs.Count() / 10);

        if (pageCount == 0)
            pageCount = 1;

        ViewBag.PageCount = pageCount;

        if (page > pageCount)
            page = pageCount;

        if (page <= 0)
            page = 1;

        ViewBag.CurrentPage = page;



        var blogs = await _context.Blogs.OrderByDescending(x=>x.Id).Skip((page-1)*10).Take(10).Include(x => x.Author).ToListAsync();

        return View(blogs);
    }


    public async Task<IActionResult> Create()
    {
        ViewBag.Topics = await _context.Topics.ToListAsync();
        ViewBag.Author = await _context.Authors.ToListAsync();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BlogCreatedto dto)
    {
        ViewBag.Topics = await _context.Topics.ToListAsync();
        ViewBag.Author = await _context.Authors.ToListAsync();
        if (!ModelState.IsValid)
            return View(dto);
        

        if (!dto.Image.CheckFileSize(2))
        {
            ModelState.AddModelError("Image", "Files cannot be more than 2mb");
            return View(dto);
        }
        if (!dto.Image.CheckFileType("image"))
        {
            ModelState.AddModelError("Image", "Files must be image type!");
            return View(dto);
        }


        var isExistAuth = await _context.Authors.AnyAsync(x => x.Id == dto.AuthorId);
        if (!isExistAuth)
        {
            ModelState.AddModelError("AuthorId", "Author is not found");
            return View(dto);
        }


        foreach (var topic in dto.TopicIds)
        {
            var isExistTopic = await _context.Topics.AnyAsync(x => x.Id == topic);
            if (!isExistTopic)
            {
                ModelState.AddModelError("TopicIds", "Topic is not found");
                return View(dto);
            }
        }

        if (_context.Blogs.Any(x => x.Title == dto.Title))
        {
            ModelState.AddModelError("", "Blog already exists");
            return View(dto);
        }
        Blog blog = _mapper.Map<Blog>(dto);

        blog.ImageUrl = await _cloudinaryService.FileCreateAsync(dto.Image);

        blog.BlogTopics = new List<BlogTopic>();

        foreach (var topicId in dto.TopicIds)
        {
            BlogTopic blogTopic = new()
            {
                Blog = blog,
                TopicId = topicId
            };
            blog.BlogTopics.Add(blogTopic);
        }

        await _context.Blogs.AddAsync(blog);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");


    }


    public async Task<IActionResult> Update(int id)
    {
        if (id < 1)
        {
            return NotFound();
        }
        ViewBag.Topics = await _context.Topics.ToListAsync();
        ViewBag.Author = await _context.Authors.ToListAsync();

        var blog = await _context.Blogs.Include(x => x.BlogTopics).Include(x => x.Author)
                                            .FirstOrDefaultAsync(x => x.Id == id);

        if (blog == null) return NotFound();

        BlogUpdatedto dto = _mapper.Map<BlogUpdatedto>(blog);
        dto.TopicIds = blog.BlogTopics.Select(x => x.TopicId).ToList();
        return View(dto);
    }


    [HttpPost]
    public async Task<IActionResult> Update(int id, BlogUpdatedto dto)
    {
        ViewBag.Topics = await _context.Topics.ToListAsync();
        ViewBag.Author = await _context.Authors.ToListAsync();

        var existBlog = await _context.Blogs.Include(x => x.BlogTopics).Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);

        if (existBlog is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }


        if (dto.Image is not null)
        {

            if (!dto.Image.CheckFileSize(2))
            {
                ModelState.AddModelError("Image", "Files cannot be more than 2mb");
                return View(dto);
            }
            if (!dto.Image.CheckFileType("image"))
            {
                ModelState.AddModelError("Image", "Files must be image type!");
                return View(dto);
            }

        }

        var isExist = await _context.Blogs.AnyAsync(x => x.Title == dto.Title && x.Id != id);
        if (isExist)
        {
            ModelState.AddModelError("", "Blog already exists");
            return View(dto);
        }


        var isExistAuth = await _context.Authors.AnyAsync(x => x.Id == dto.AuthorId);
        if (!isExistAuth)
        {
            ModelState.AddModelError("AuthorId", "Author is not found");
            return View(dto);
        }


        foreach (var topic in dto.TopicIds)
        {
            var isExistTopic = await _context.Topics.AnyAsync(x => x.Id == topic);
            if (!isExistTopic)
            {
                ModelState.AddModelError("TopicIds", "Topic is not found");
                return View(dto);
            }
        }


        foreach (var blogTopic in existBlog.BlogTopics)
        {
            _context.BlogTopics.Remove(blogTopic);
        }

        foreach (var topic in dto.TopicIds)
        {
            BlogTopic bTopic = new() { Blog = existBlog, TopicId = topic };
            existBlog.BlogTopics.Add(bTopic);
        }




        existBlog = _mapper.Map(dto, existBlog);

    if(dto.Image is not null)
    {
        existBlog.ImageUrl = await _cloudinaryService.FileCreateAsync(dto.Image);
    }

        _context.Update(existBlog);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var blog = await _context.Blogs.FirstOrDefaultAsync(x => x.Id == id);

        if (blog is null)
            return NotFound();

        blog.IsDeleted = true;

        _context.Blogs.Update(blog);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}

