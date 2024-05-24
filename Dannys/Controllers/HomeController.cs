using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Dannys.Controllers;

public class HomeController : Controller
{

    public IActionResult Index()
    {
        return View();
    }
} 

