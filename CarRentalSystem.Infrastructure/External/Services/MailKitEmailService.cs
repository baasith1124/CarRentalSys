using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.External.Services
{
    public class MailKitEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public MailKitEmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                Console.WriteLine($"MailKitEmailService: Starting to send email to {to}");
                Console.WriteLine($"MailKitEmailService: From: {_settings.FromEmail}");
                Console.WriteLine($"MailKitEmailService: SMTP Server: {_settings.SmtpServer}:{_settings.SmtpPort}");
                
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = body };

                using var smtp = new SmtpClient();
                Console.WriteLine($"MailKitEmailService: Connecting to SMTP server...");
                await smtp.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
                
                Console.WriteLine($"MailKitEmailService: Authenticating...");
                await smtp.AuthenticateAsync(_settings.FromEmail, _settings.Password);
                
                Console.WriteLine($"MailKitEmailService: Sending email...");
                await smtp.SendAsync(email);
                
                Console.WriteLine($"MailKitEmailService: Disconnecting...");
                await smtp.DisconnectAsync(true);
                
                Console.WriteLine($"MailKitEmailService: Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MailKitEmailService Exception: {ex.Message}");
                Console.WriteLine($"MailKitEmailService Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
    }

}
