using AutoMapper;
using Dannys.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CategoryController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int page=1)
    {
        int pageCount = (int)Math.Ceiling((decimal)_context.Categories.Count() / 10);


        if (pageCount == 0)
            pageCount = 1;
        
        ViewBag.PageCount = pageCount;


        if (page > pageCount)
            page = pageCount;

        if (page <= 0)
            page = 1;

        ViewBag.CurrentPage = page;

        var categories = await _context.Categories.Skip((page-1)*10).Take(10).Include(x => x.Products).OrderBy(x=>x.Order).ToListAsync();

        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateDto dto)
    {

        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        var isExistTitle = await _context.Categories.AnyAsync(x => x.Title.ToLower() == dto.Title.ToLower() );
        var isExistSubTitle = await _context.Categories.AnyAsync(x => x.SubTitle.ToLower() == dto.SubTitle.ToLower() );

        if (isExistTitle)
        {
            ModelState.AddModelError("Title", "Title alredy exist");
            return View(dto);
        }
        if (isExistSubTitle)
        {
            ModelState.AddModelError("SubTitle", "Subtitle alredy exist");
            return View(dto);
        }

        Category category = _mapper.Map<Category>(dto);

       

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
            return NotFound();

        CategoryUpdateDto dto = _mapper.Map<CategoryUpdateDto>(category);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
    {

        var existCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (existCategory is null)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(dto);

        var isExistTitle = await _context.Categories.AnyAsync(x => x.Title.ToLower() == dto.Title.ToLower() && x.Id != id);
        var isExistSubTitle = await _context.Categories.AnyAsync(x => x.SubTitle.ToLower() == dto.SubTitle.ToLower() && x.Id != id);

        if (isExistTitle)
        {
            ModelState.AddModelError("Title", "Title alredy exist");
            return View(dto);
        }
        if (isExistSubTitle)
        {
            ModelState.AddModelError("SubTitle", "Subtitle alredy exist");
            return View(dto);
        }

        existCategory = _mapper.Map(dto, existCategory);

        

        _context.Update(existCategory);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {

        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (category is null)
            return NotFound();

        category.IsDeleted= true;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}

