using Dannys.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Dannys.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]

public class DashboardController : Controller
{
  

    public IActionResult Index()
    {
        return View();
    }

}


