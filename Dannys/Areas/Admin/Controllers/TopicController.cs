using Dannys.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
public class TopicController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TopicController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var topics = await _context.Topics.Where(x => !x.IsDeleted).ToListAsync()   ;
        return View(topics);
    }
    public IActionResult Create()
    { 
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TopicCreateDto dto)
    {
       
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        
        var isExist = await _context.Topics.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower());

        if (isExist)
        {
            ModelState.AddModelError("Name", "Topic already exist");
            return View(dto);
        }


        Topic topic = _mapper.Map<Topic>(dto);

        await _context.Topics.AddAsync(topic);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var topic = await _context.Topics.FirstOrDefaultAsync(x => x.Id == id);

        if (topic is null)
            return NotFound();
        


        TopicUpdateDto dto =_mapper.Map<TopicUpdateDto>(topic);
        return View(dto);

    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, TopicUpdateDto dto)
    {
        var existTopic = await _context.Topics.FirstOrDefaultAsync(x => x.Id == id);
        if (existTopic is null)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(dto);

        var isExist = await _context.Topics.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower() && x.Id != id);

        if (isExist)
        {
            ModelState.AddModelError("Title", "Title alredy exist");
            return View(dto);
        }

        existTopic = _mapper.Map(dto, existTopic);



        _context.Update(existTopic);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");

    }

    public async Task<IActionResult> Delete(int id)
    {

        var topic = await _context.Topics.FirstOrDefaultAsync(x => x.Id == id);

        if (topic is null)
            return NotFound();
        topic.IsDeleted = true;
        _context.Topics.Update(topic);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");

    }


}

