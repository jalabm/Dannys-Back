using System;
using AutoMapper;
using Dannys.Data;
using Dannys.Extensions;
using Dannys.Models;
using Dannys.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
public class SliderController : Controller
{

    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;
    private readonly CloudinaryService _cloudinaryService;


    public SliderController(AppDbContext context, IWebHostEnvironment env, IMapper mapper, CloudinaryService cloudinaryService)
    {
        _context = context;
        _env = env;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<IActionResult> Index()
    {

        var sliders = await _context.Sliders.ToListAsync();
        return View(sliders);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SliderCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        if (!dto.File.CheckFileType("image"))
        {
            ModelState.AddModelError("", "Invalid File");
            return View(dto);
        }
        if (!dto.File.CheckFileSize(2))
        {
            ModelState.AddModelError("", "Invalid File Size");
            return View(dto);
        }

        Slider slider = _mapper.Map<Slider>(dto);
        slider.Url = await _cloudinaryService.FileCreateAsync(dto.File);
        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var slider = await _context.Sliders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (slider is null)
        {
            return NotFound();
        }

        SliderUpdateDto dto = _mapper.Map<SliderUpdateDto>(slider);
        return View(dto);

    }


    [HttpPost]
    public async Task<IActionResult> Update(int id, SliderUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var existSlider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);

        if (existSlider is null)
            return NotFound();

        if (dto.File is not null)
        {
            if (!dto.File.CheckFileType("image"))
            {
                ModelState.AddModelError("File", "Invalid File Type");
                return View(dto);
            }
            if (!dto.File.CheckFileSize(2))
            {
                ModelState.AddModelError("File", "Invalid File Size");
                return View(dto);
            }
            existSlider.Url.DeleteFile(_env.WebRootPath, "assets", "image", "sliderIcons");

            if (dto.File is not null)
            {
                existSlider.Url = await _cloudinaryService.FileCreateAsync(dto.File);
            }
           
        }

        existSlider = _mapper.Map(dto, existSlider);
        _context.Update(existSlider);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }


    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var slider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);


        if (slider is null)
        {
            return NotFound();
        }
        _context.Sliders.Remove(slider);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}

