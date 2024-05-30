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

            if (reservation.IsDone)
                tables.Add(table);
        }

        var dtos = _mapper.Map<List<TableGetDto>>(tables);


        for (int i = 0; i < dtos.Count; i++)
        {
            var table = tables[i];


            var reservation = table.Reservations.OrderBy(x=>x.Date).FirstOrDefault(y => y.Date.Date == date);


            if(reservation is not null && !reservation.IsDone)
            {
                dtos[i].ReservInfo = $"{reservation.Date.Hour}:{reservation.Date.Minute} reserved";
            }

        }

        return dtos;
    }
    [HttpPost]
    public async Task<IActionResult> Index(ReservationCreateDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        if (dto.Time < DateTime.UtcNow.AddHours(2))
        {
            ModelState.AddModelError("Time", "You can only reservate 2 hours ago");
            return View(dto);
        }

        var existTable = await _context.Tables.Include(x=>x.Reservations).FirstOrDefaultAsync(x => x.Id == dto.TableId);

        if(existTable is null)
        {
            ModelState.AddModelError("TableId", "Table is not found");
            return View(dto);
        }


        var existReservation = existTable.Reservations.OrderBy(x => x.Date).FirstOrDefault(x => x.Date.Date == dto.Time.Date);

        if(existReservation is not null)
        {
            if (existReservation.Date < dto.Time.AddHours(2))
            {
                ModelState.AddModelError("", "Table is not found pls choose correct table");
                return View(dto);
            }
        }


        Reservation reservation = new()
        {
            Name = dto.Name,
            Email = dto.PhoneNumber,
            Date = dto.Time,
            TableId = dto.TableId
        };
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();


        TempData["message"] = "Reservation is successfully done.";

        return RedirectToAction("Index");
    }
}

