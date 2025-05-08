using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ServerApp.Utilities
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public EmailService(string smtpServer, int port, string username, string password)
        {
            _smtpServer = smtpServer;
            _port = port;
            _username = username;
            _password = password;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using (var smtpClient = new SmtpClient(_smtpServer)
            {
                Port = _port,
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true,
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_username),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(to);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}