using MailKit.Security;
using Microsoft.AspNetCore.Server.IIS.Core;
using MimeKit.Text;
using MimeKit;
using ProjetoC_.Models;
using MailKit.Net.Smtp;

namespace ProjetoC_.Servicos.EmailService
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public IConfiguration Config { get; }

        public void SendEmail(EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUserName").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using var smpt = new SmtpClient();
            smpt.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smpt.Authenticate(_config.GetSection("EmailUserName").Value, _config.GetSection("EmailPassword").Value);
            smpt.Send(email);
            smpt.Disconnect(true);
        }
    }
}
