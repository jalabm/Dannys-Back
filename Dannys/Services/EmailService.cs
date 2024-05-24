using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Dannys.Services
{
    public class EmailService : IEmailService
    {


        public void SendEmail(string toEmail, string subject, string body)
        {
            // Set up SMTP client
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("jalabm@code.edu.az", "qonj cnsp zmjn awxk");




            // Create email message
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("jalabm@code.edu.az");
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;

            StringBuilder mailBody = new StringBuilder();

            mailBody.AppendFormat(body);

            mailMessage.Body = mailBody.ToString();


            client.Send(mailMessage);
        }

    }


    public interface IEmailService
    {
        void SendEmail(string toEmail, string subject, string body);
    }
}

