using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace ProjetoC_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendMail(string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("king.wehner27@ethereal.email"));
            email.To.Add(MailboxAddress.Parse("king.wehner27@ethereal.email"));
            email.Subject = "Test email subject";
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smpt = new SmtpClient();
            smpt.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smpt.Authenticate("king.wehner27@ethereal.email", "q2QBTKkSRz3R7w92XQ");
            smpt.Send(email);
            smpt.Disconnect(true);

            return Ok();
        }
    }
}
