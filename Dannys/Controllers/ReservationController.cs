using Dannys.Data;
using Dannys.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Controllers;

public class ReservationController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;


    public ReservationController(AppDbContext context, IMapper mapper, IEmailService emailService)
    {
        _context = context;
        _mapper = mapper;
        _emailService = emailService;
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
            var reservation = table.Reservations.OrderBy(x=>x.Date).FirstOrDefault(y => y.Date.Date == date);

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
            ModelState.AddModelError("Time", "You can only reserve 2 hours ago");
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
                ModelState.AddModelError("", "Table is not found please choose correct table");
                return View(dto);
            }
        }


        Reservation reservation = new()
        {
            Name = dto.Name,
            Email = dto.Email,
            Date = dto.Time,
            TableId = dto.TableId
        };
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
        string body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reservation Confirmation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}
        .email-header, .email-footer {{
            background-color: #4CAF50;
            color: white;
            text-align: center;
            padding: 10px;
        }}
        .email-body {{
            padding: 20px;
            color: #333;
        }}
        .email-body h2 {{
            color: #333;
        }}
        .reservation-details {{
            margin: 20px 0;
        }}
        .reservation-details table {{
            width: 100%;
            border-collapse: collapse;
        }}
        .reservation-details th, .reservation-details td {{
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }}
        .reservation-details th {{
            background-color: #f2f2f2;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <h1>Reservation Confirmation</h1>
        </div>
        <div class='email-body'>
            <h2>Dear {dto.Name},</h2>
            <p>Thank you for your reservation. Here are your booking details:</p>
            <div class='reservation-details'>
                <table>
                    <tr>
                        <th>Reservation Number</th>
                        <td>{reservation.Id}</td>
                    </tr>
                    <tr>
                        <th>Date</th>
                        <td>{reservation.Date.ToShortDateString()}</td>
                    </tr>
                    <tr>
                        <th>Time</th>
                        <td>{reservation.Date.ToShortTimeString()}</td>
                    </tr>
                    <tr>
                        <th>Table</th>
                        <td>{existTable.TableNo}</td>
                    </tr>
                    <tr>
                        <th>Guests</th>
                        <td>{existTable.PersonCount}</td>
                    </tr>
                </table>
            </div>
            <p>We look forward to serving you!</p>
            <p>Best regards,<br>The Dannys Team</p>
        </div>
        <div class='email-footer'>
            <p>&copy; 2003 Dannys Restaurant. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
";

        // Send the email
        _emailService.SendEmail(dto.Email, "Reservation detail", body);

        TempData["message"] = "Reservation is successfully done.";

        return RedirectToAction("Index");
    }
}

