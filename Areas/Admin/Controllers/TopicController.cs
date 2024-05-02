using Microsoft.AspNetCore.Mvc;


namespace Dannys.Areas.Admin.Controllers;

public class TopicController : Controller
{
    public IActionResult Index()
    {
        return View();
    }


}

