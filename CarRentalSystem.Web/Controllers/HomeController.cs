using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Common.Settings;
using CarRentalSystem.Web.Models;
using CarRentalSystem.Web.ViewModels.Car;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;


namespace CarRentalSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;
        private readonly EmailSettings _emailSettings;
        private readonly GoogleSettings _googleSettings;

        public HomeController(
            ILogger<HomeController> logger,
            ICarRepository carRepository,
            IMapper mapper,
            IConfiguration configuration,
            IOptions<GoogleSettings> googleSettings)
        {
            _logger = logger;
            _carRepository = carRepository;
            _mapper = mapper;
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
            _googleSettings = googleSettings.Value;
        }

        public async Task<IActionResult> Index(string? error = null)
        {
            var cars = await _carRepository.GetTopAvailableCarsAsync();
            var carViewModels = _mapper.Map<List<CarViewModel>>(cars);
            
            // Handle error messages from Google authentication
            if (!string.IsNullOrEmpty(error))
            {
                TempData["Error"] = error;
            }
            
            // Pass Google Places API key to the view
            ViewBag.GooglePlacesApiKey = _googleSettings.PlacesApiKey;
            
            return View(carViewModels);
        }




        [HttpPost]
        public async Task<IActionResult> SendContactEmail([FromBody] ContactEmailRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SenderName) || 
                    string.IsNullOrEmpty(request.SenderEmail) || 
                    string.IsNullOrEmpty(request.MessageSubject) || 
                    string.IsNullOrEmpty(request.MessageContent))
                {
                    return Json(new { success = false, message = "All fields are required." });
                }

                // Send email to admin
                await SendEmailToAdmin(request);
                
                // Send confirmation email to user
                await SendConfirmationEmailToUser(request);

                return Json(new { success = true, message = "Message sent successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending contact email");
                return Json(new { success = false, message = "Failed to send message. Please try again." });
            }
        }

        private async Task SendEmailToAdmin(ContactEmailRequest request)
        {
            var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.Password)
            };

            var adminEmail = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = $"New Contact Query: {request.MessageSubject}",
                IsBodyHtml = true,
                Body = GenerateAdminEmailHtml(request)
            };

            adminEmail.To.Add(_emailSettings.SupportEmail);
            adminEmail.ReplyToList.Add(new MailAddress(request.SenderEmail, request.SenderName));

            await smtpClient.SendMailAsync(adminEmail);
        }

        private async Task SendConfirmationEmailToUser(ContactEmailRequest request)
        {
            var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.Password)
            };

            var userEmail = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = "Thank you for contacting us - We'll connect with you soon!",
                IsBodyHtml = true,
                Body = GenerateUserConfirmationEmailHtml(request)
            };

            userEmail.To.Add(request.SenderEmail);

            await smtpClient.SendMailAsync(userEmail);
        }

        private string GenerateAdminEmailHtml(ContactEmailRequest request)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 0; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.2); }}
        .header {{ background: linear-gradient(135deg, #3B82F6 0%, #1E40AF 100%); color: white; padding: 2rem; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 1.8rem; }}
        .content {{ padding: 2rem; }}
        .field {{ margin-bottom: 1.5rem; }}
        .field-label {{ font-weight: 600; color: #374151; margin-bottom: 0.5rem; display: block; }}
        .field-value {{ background: #f8fafc; padding: 1rem; border-radius: 8px; border-left: 4px solid #3B82F6; }}
        .message-content {{ background: #f8fafc; padding: 1.5rem; border-radius: 8px; border-left: 4px solid #10B981; white-space: pre-wrap; }}
        .footer {{ background: #f8fafc; padding: 1.5rem; text-align: center; color: #6B7280; border-top: 1px solid #E5E7EB; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🚗 New Contact Query</h1>
            <p>Someone has contacted you through your website</p>
        </div>
        <div class='content'>
            <div class='field'>
                <span class='field-label'>👤 From:</span>
                <div class='field-value'>{request.SenderName}</div>
            </div>
            <div class='field'>
                <span class='field-label'>📧 Email:</span>
                <div class='field-value'>{request.SenderEmail}</div>
            </div>
            <div class='field'>
                <span class='field-label'>📝 Subject:</span>
                <div class='field-value'>{request.MessageSubject}</div>
            </div>
            <div class='field'>
                <span class='field-label'>💬 Message:</span>
                <div class='message-content'>{request.MessageContent}</div>
            </div>
        </div>
        <div class='footer'>
            <p>This message was sent from your Car Rental System website contact form.</p>
            <p>Reply directly to this email to respond to the customer.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerateUserConfirmationEmailHtml(ContactEmailRequest request)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 0; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.2); }}
        .header {{ background: linear-gradient(135deg, #10B981 0%, #059669 100%); color: white; padding: 2rem; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 1.8rem; }}
        .content {{ padding: 2rem; }}
        .message {{ background: #f0fdf4; padding: 1.5rem; border-radius: 8px; border-left: 4px solid #10B981; margin-bottom: 1.5rem; }}
        .contact-info {{ background: #f8fafc; padding: 1.5rem; border-radius: 8px; border: 1px solid #E5E7EB; }}
        .contact-item {{ display: flex; align-items: center; margin-bottom: 0.75rem; }}
        .contact-item:last-child {{ margin-bottom: 0; }}
        .contact-icon {{ width: 20px; margin-right: 0.75rem; color: #3B82F6; }}
        .footer {{ background: #f8fafc; padding: 1.5rem; text-align: center; color: #6B7280; border-top: 1px solid #E5E7EB; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✅ Message Received!</h1>
            <p>Thank you for contacting Car Rental System</p>
        </div>
        <div class='content'>
            <div class='message'>
                <h3>Hello {request.SenderName},</h3>
                <p>We have received your message regarding <strong>""{request.MessageSubject}""</strong> and we appreciate you reaching out to us.</p>
                <p>Our team will review your inquiry and get back to you within 2 hours during business hours. We're committed to providing you with the best car rental experience.</p>
            </div>
            <div class='contact-info'>
                <h4>Need immediate assistance?</h4>
                <div class='contact-item'>
                    <span class='contact-icon'>📞</span>
                    <span>Call us: <strong>+94 77 123 4567</strong></span>
                </div>
                <div class='contact-item'>
                    <span class='contact-icon'>💬</span>
                    <span>WhatsApp: <strong>+94 77 735 3481</strong></span>
                </div>
                <div class='contact-item'>
                    <span class='contact-icon'>📧</span>
                    <span>Email: <strong>{_emailSettings.SupportEmail}</strong></span>
                </div>
            </div>
        </div>
        <div class='footer'>
            <p>Best regards,<br><strong>Car Rental System Team</strong></p>
            <p>This is an automated confirmation. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
