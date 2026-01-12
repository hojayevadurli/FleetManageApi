using FleetManage.Api.Interfaces;
using System.Net;
using System.Net.Mail;

namespace FleetManage.Api.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var host = _config["Email:Host"];
            var portVal = _config["Email:Port"];
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var from = _config["Email:From"] ?? username;

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Email sending skipped: Missing SMTP configuration (Email:Host, Email:Username, or Email:Password).");
                return;
            }

            int port = 587; // default
            if (int.TryParse(portVal, out int p)) port = p;

            try
            {
                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from!),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent to {To}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", toEmail);
                throw; // Re-throw so the controller knows it failed? Or suppress?
                       // Usually good to throw so we see the error 500 in dev, or handle gracefuly.
                       // For now, let's just log. The user might want to know if it failed.
            }
        }
    }
}
