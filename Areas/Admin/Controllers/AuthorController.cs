using Dannys.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthorController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AuthorController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var authors = await _context.Authors.Where(x => !x.IsDeleted).ToListAsync();
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

        Author author = _mapper.Map<Author>(dto);



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

        var isExistBiographia = await _context.Authors.AnyAsync(x => x.Biographia.ToLower() == dto.Biographia.ToLower()&&x.Id != id);

        if (isExistBiographia)
        {
            ModelState.AddModelError("Biographia", "Biographia already exist");
            return View(dto);
        }

        existAuthor = _mapper.Map(dto, existAuthor);

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

