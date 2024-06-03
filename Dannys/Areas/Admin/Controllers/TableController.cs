using Dannys.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
public class TableController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;


    public TableController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }



    public async Task<IActionResult> Index(int page = 1)
    {

        int pageCount = (int)Math.Ceiling((decimal)_context.Tables.Count() / 10);

        if (pageCount == 0)
            pageCount = 1;

        ViewBag.PageCount = pageCount;

        if (page > pageCount)
            page = pageCount;

        if (page <= 0)
            page = 1;

        ViewBag.CurrentPage = page;

        var tables = await _context.Tables.OrderByDescending(x => x.Id).Skip((page - 1) * 10).Take(10).ToListAsync();
        return View(tables);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(TableCreateDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var isExist = await _context.Tables.AnyAsync(x => x.TableNo == dto.TableNo);

        if (isExist)
        {
            ModelState.AddModelError("TableNo", "Table already exist");
            return View(dto);
        }
        var allowedPersonCount = ( dto.PersonCount != 2 && dto.PersonCount != 4 && dto.PersonCount != 6 && dto.PersonCount != 8 && dto.PersonCount != 10);

        if (allowedPersonCount)
        {
            ModelState.AddModelError("PersonCount", "Person Count can only 2,4,6,8,10");
            return View(dto);

        }

        Table table = _mapper.Map<Table>(dto);

        await _context.Tables.AddAsync(table);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var table = await _context.Tables.FirstOrDefaultAsync(x => x.Id == id);

        if (table is null)
            return NotFound();



        TableGetDto dto = _mapper.Map<TableGetDto>(table);
        return View(dto);

    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, TableGetDto dto)
    {
        var existTable = await _context.Tables.FirstOrDefaultAsync(x => x.Id == id);
        if (existTable is null)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(dto);

        var isExist = await _context.Tables.AnyAsync(x => x.TableNo == dto.TableNo);

        if (isExist)
        {
            ModelState.AddModelError("TableNo", "Table already exist");
            return View(dto);
        }

        var allowedPersonCount = (dto.PersonCount != 2 && dto.PersonCount != 4 && dto.PersonCount != 6 && dto.PersonCount != 8 && dto.PersonCount != 10);

        if (allowedPersonCount)
        {
            ModelState.AddModelError("PersonCount", "Person Count can only 2,4,6,8,10");
            return View(dto);

        }

        existTable = _mapper.Map(dto, existTable);

        _context.Tables.Update(existTable);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");

    }

    public async Task<IActionResult> Delete(int id)
    {

        var table = await _context.Tables.FirstOrDefaultAsync(x => x.Id == id);

        if (table is null)
            return NotFound();
       
        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");

    }

}

