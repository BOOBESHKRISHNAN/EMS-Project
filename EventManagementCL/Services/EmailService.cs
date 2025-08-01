using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;


namespace EventManagementCL.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string bodyHtml)
        {
            var settings = _config.GetSection("SmtpSettings");

            using var client = new SmtpClient(settings["Host"], int.Parse(settings["Port"]))
            {
                EnableSsl = bool.Parse(settings["EnableSSL"]),
                Credentials = new NetworkCredential(settings["Username"], settings["Password"])
            };

            var mail = new MailMessage
            {
                From = new MailAddress(settings["Username"], "EventEase Notification"),
                Subject = subject,
                Body = bodyHtml,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            try
            {
                await client.SendMailAsync(mail);
                _logger.LogInformation("Email sent to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
            }
        }
    }
}
