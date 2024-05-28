using Microsoft.AspNetCore.Mvc;

namespace Dannys.Controllers;

public class ContactController : Controller
{
    public IActionResult Index()
    {

        return View();
    }
}

