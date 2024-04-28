using Microsoft.AspNetCore.Mvc;


namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

