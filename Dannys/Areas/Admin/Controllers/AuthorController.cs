using Dannys.Data;
using Dannys.Extensions;
using Dannys.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthorController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly CloudinaryService _cloudinaryService;

    public AuthorController(AppDbContext context, IMapper mapper, CloudinaryService cloudinaryService)
    {
        _context = context;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        int pageCount = (int)Math.Ceiling((decimal)_context.Authors.Count() / 10);

        if (pageCount == 0)
            pageCount = 1;

        ViewBag.PageCount = pageCount;

        if (page > pageCount)
            page = pageCount;

        if (page <= 0)
            page = 1;

        ViewBag.CurrentPage = page;

        var authors = await _context.Authors.OrderByDescending(x=>x.Id).Skip((page-1)*10).Take(10).ToListAsync();
        return View(authors);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(AuthorCreateDto dto)
    {

        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        var isExistDescription = await _context.Authors.AnyAsync(x => x.Description.ToLower() == dto.Description.ToLower());
        var isExistBiographia = await _context.Authors.AnyAsync(x => x.Biographia.ToLower() == dto.Biographia.ToLower());


        if (isExistDescription)
        {
            ModelState.AddModelError("Description", "Description already exist");
            return View(dto);
        }
        if (isExistBiographia)
        {
            ModelState.AddModelError("Biographia", "Biographia already exist");
            return View(dto);
        }

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

        Author author = _mapper.Map<Author>(dto);
        author.ImageUrl = await _cloudinaryService.FileCreateAsync(dto.Image);


        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);

        if (author is null)
            return NotFound();

        AuthorUpdateDto dto = _mapper.Map<AuthorUpdateDto>(author);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, AuthorUpdateDto dto)
    {

        var existAuthor = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);
        if (existAuthor is null)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(dto);

        var isExistBiographia = await _context.Authors.AnyAsync(x => x.Biographia.ToLower() == dto.Biographia.ToLower() && x.Id != id);

        if (isExistBiographia)
        {
            ModelState.AddModelError("Biographia", "Biographia already exist");
            return View(dto);
        }


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

        existAuthor = _mapper.Map(dto, existAuthor);
        if (dto.Image is not null)
        {
            existAuthor.ImageUrl = await _cloudinaryService.FileCreateAsync(dto.Image);
        }
        _context.Update(existAuthor);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {

        var author = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);
        if (author is null)
            return NotFound();

        author.IsDeleted = true;
        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}

