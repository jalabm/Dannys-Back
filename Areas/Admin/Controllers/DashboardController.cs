using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]

public class DashboardController : Controller
{
    private readonly IMapper _mapper;

    public DashboardController(IMapper mapper)
    {
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        List<Product> products = new();


        var dtos = _mapper.Map<List<ProductGetDto>>(products);


        return View(dtos);
    }

    public IActionResult Create(int id,ProductUpdateDto dto)
    {
        Product existProduct=new();


        var product = _mapper.Map(existProduct,dto);//update

        var product2 = _mapper.Map<Product>(dto);//create


        return RedirectToAction("index");
    }
}


