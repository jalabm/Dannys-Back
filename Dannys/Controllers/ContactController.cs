using Dannys.Services;
using Dannys.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Dannys.Controllers;

public class ContactController : Controller
{

    private readonly IEmailService _emailService;

    public ContactController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public IActionResult Index()
    {

        return View();
    }
    [HttpPost]
    public IActionResult Index(ContactVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);


        string emailBody = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Contact Information</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
            color: #333;
            margin: 0;
            padding: 0;
        }}

        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border: 1px solid #ddd;
            border-radius: 5px;
            overflow: hidden;
        }}

        .header {{
            background-color: #cc9933;
            color: #ffffff;
            text-align: center;
            padding: 20px;
        }}

        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}

        .content {{
            padding: 20px;
        }}

        .content p {{
            margin: 10px 0;
        }}

        .footer {{
            text-align: center;
            padding: 20px;
            background-color: #f2f2f2;
            color: #666;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Contact Information</h1>
        </div>
        <div class=""content"">
            <p>Hello,</p>
            <p>Here is the contact information you requested:</p>
            <p><strong>Fullname:</strong> {vm.Fullname}</p>
            <p><strong>Phone Number:</strong> {vm.PhoneNumber}</p>
            <p><strong>Email:</strong> {vm.Email}</p>
            <p><strong>Message:</strong> {vm.Message}</p>
        </div>
        <div class=""footer"">
            <p>Thank you!</p>
        </div>
    </div>
</body>
</html>";


        _emailService.SendEmail("jalabm@code.edu.az","Contact info",emailBody);

        TempData["message"] = "Email sent successfully";

        return RedirectToAction("Index");
    }
}

