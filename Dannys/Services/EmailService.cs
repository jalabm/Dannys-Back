using System.Net;
using System.Net.Mail;

namespace Dannys.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //public void SendEmail(string toEmail, string subject, string body)
    //{
    //    // Set up SMTP client
    //    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
    //    client.EnableSsl = true;
    //    client.UseDefaultCredentials = false;
    //    client.Credentials = new NetworkCredential("jalabm@code.edu.az", "qonj cnsp zmjn awxk");




    //    // Create email message
    //    MailMessage mailMessage = new MailMessage();
    //    mailMessage.From = new MailAddress("jalabm@code.edu.az");
    //    mailMessage.To.Add(toEmail);
    //    mailMessage.Subject = subject;
    //    mailMessage.IsBodyHtml = true;

      

    //    mailMessage.Body = body;


    //    client.Send(mailMessage);
    //}

    public void SendEmail(string toEmail, string subject, string body)
    {
        SmtpClient smtpClient = new SmtpClient(_configuration["EmailSettings:Host"], int.Parse(_configuration["EmailSettings:Port"]))
        {
            Credentials = new NetworkCredential(_configuration["EmailSettings:Email"], _configuration["EmailSettings:Password"]),
            EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]),
        };
        MailMessage mailMessage = new MailMessage()
        {
            Subject = subject,
            From = new MailAddress(_configuration["EmailSettings:Email"]),
            IsBodyHtml = bool.Parse(_configuration["EmailSettings:IsHtml"]),
        };
        mailMessage.To.Add(toEmail);

        mailMessage.Body = body;

        smtpClient.Send(mailMessage);
    }
}


public interface IEmailService
{
    void SendEmail(string toEmail, string subject, string body);
}

