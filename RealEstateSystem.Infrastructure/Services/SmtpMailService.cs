using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Infrastructure.Services
{
    public class SmtpMailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public SmtpMailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
        {
            var smtpHost = _configuration["Smtp:Host"]
                ?? throw new InvalidOperationException("Cấu hình SMTP Host không tồn tại.");
            var smtpPortText = _configuration["Smtp:Port"]
                ?? throw new InvalidOperationException("Cấu hình SMTP Port không tồn tại.");
            var smtpUsername = _configuration["Smtp:Username"]
                ?? throw new InvalidOperationException("Cấu hình SMTP Username không tồn tại.");
            var smtpPassword = _configuration["Smtp:Password"]
                ?? throw new InvalidOperationException("Cấu hình SMTP Password không tồn tại.");
            var displayName = _configuration["Smtp:DisplayName"] ?? "Real Estate System";
            var fromEmail = _configuration["Smtp:FromEmail"] ?? smtpUsername;

            if (!int.TryParse(smtpPortText, out var smtpPort))
            {
                throw new InvalidOperationException("Cấu hình SMTP Port không hợp lệ.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(displayName, fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await smtpClient.AuthenticateAsync(smtpUsername, smtpPassword, cancellationToken);
            await smtpClient.SendAsync(message, cancellationToken);
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }
    }
}
