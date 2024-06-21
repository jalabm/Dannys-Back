using Dannys.Data;
using Dannys.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CouponController : Controller
{

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CouponController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int page=1)
    {

        int pageCount = (int)Math.Ceiling((decimal)_context.Coupons.Count() / 10);

        if (pageCount == 0)
            pageCount = 1;

        ViewBag.PageCount = pageCount;

        if (page > pageCount)
            page = pageCount;

        if (page <= 0)
            page = 1;

        ViewBag.CurrentPage = page;

        var coupon = await _context.Coupons.Skip((page-1)*10).Take(10).ToListAsync();
        return View(coupon);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CouponCreateDto dto)
    {

        if (!ModelState.IsValid)
        {
            return View(dto);
        }


        var isExist = await _context.Coupons.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower());

        if (isExist)
        {
            ModelState.AddModelError("Name", "Coupon already exist");
            return View(dto);
        }


        Coupon coupon = _mapper.Map<Coupon>(dto);

        await _context.Coupons.AddAsync(coupon);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Update(int id)
    {
        var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == id);

        if (coupon is null)
            return NotFound();



        CouponUpdateDto dto = _mapper.Map<CouponUpdateDto>(coupon);
        return View(dto);

    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, CouponUpdateDto dto)
    {
        var existCoupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == id);
        if (existCoupon is null)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(dto);

        var isExist = await _context.Coupons.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower() && x.Id != id);

        if (isExist)
        {
            ModelState.AddModelError("Name", "Name alredy exist");
            return View(dto);
        }

        existCoupon = _mapper.Map(dto, existCoupon);



        _context.Update(existCoupon);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");

    }

    public async Task<IActionResult> Delete(int id)
    {

        var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == id);

        if (coupon is null)
            return NotFound();
        coupon.IsDeleted = true;
        _context.Coupons.Update(coupon);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");

    }
}
