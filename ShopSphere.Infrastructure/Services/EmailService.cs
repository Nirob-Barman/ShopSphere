using Microsoft.Extensions.Options;
using ShopSphere.Application.Interfaces.Email;
using ShopSphere.Application.Settings.Email;
using System.Net;
using System.Net.Mail;

namespace ShopSphere.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
                    throw new Exception("SenderEmail is not configured.");

                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = _emailSettings.EnableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail!, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send email", ex);
            }
        }
    }
}
