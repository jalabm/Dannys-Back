using Dannys.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Controllers;

public class ReservationController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ReservationController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<List<TableGetDto>> GetTablesByPersonCount(int count, DateTime time)
    {
        var date = time.Date;
        var tables = await _context.Tables
            .Include(x => x.Reservations)
            .Where(x => x.PersonCount == count && !x.Reservations.Any(y => y.Date.Date == date ))
            .ToListAsync();


        var reservedTables = await _context.Tables.Include(x => x.Reservations).Where(x => x.PersonCount == count && x.Reservations.Any(y => y.Date.Date == date)).ToListAsync();


        foreach (var table in reservedTables)
        {
            var reservation = table.Reservations.FirstOrDefault(y => y.Date.Date == date);

            if (reservation is null)
                continue;

            if (time.AddHours(2) < reservation.Date)
            {
                tables.Add(table);
            }
        }

        var dtos = _mapper.Map<List<TableGetDto>>(tables);

        return dtos;
    }
    [HttpPost]
    public async Task<IActionResult> Reserve(ReservationCreateDto dto)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Index");

        Reservation reservation = new()
        {
            Name = dto.Name,
            Email = dto.PhoneNumber,
            Date = dto.Time,
            TableId = dto.TableId
        };
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}

