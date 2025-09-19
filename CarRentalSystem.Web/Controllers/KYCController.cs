using CarRentalSystem.Application.Features.KYC.Command.UploadKYC;
using CarRentalSystem.Web.Extensions;
using CarRentalSystem.Web.ViewModels.KYC;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Web.Controllers
{
    [Authorize]
    [Route("KYC")]
    public class KYCController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;

        public KYCController(IMediator mediator, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _env = env;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View(new KYCUploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Upload(KYCUploadViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Save file to /wwwroot/kyc/
            var kycFolder = Path.Combine(_env.WebRootPath, "kyc");
            Directory.CreateDirectory(kycFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Document.FileName);
            var filePath = Path.Combine(kycFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Document.CopyToAsync(stream);
            }

            // Send command to save KYC record
            var command = new UploadKYCCommand
            {
                UserId = User.GetUserId(),
                DocumentType = model.DocumentType,
                FilePath = $"/kyc/{fileName}"
            };

            await _mediator.Send(command);

            TempData["Success"] = "KYC document uploaded successfully. Please wait for admin approval.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok(new { success = true, message = "KYC controller is working" });
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return Ok(new { success = true, message = "KYC controller index" });
        }

        [HttpPost("UploadAjax")]
        public async Task<IActionResult> UploadAjax(KYCUploadViewModel model)
        {
            try
            {
                // Log the incoming request
                Console.WriteLine($"KYC Upload Request - DocumentType: {model?.DocumentType}, File: {model?.Document?.FileName}, Size: {model?.Document?.Length}");
                Console.WriteLine($"Model is null: {model == null}");
                
                if (model == null)
                {
                    Console.WriteLine("Model is null");
                    return BadRequest(new { success = false, message = "No data received" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    Console.WriteLine($"Model validation failed: {string.Join(", ", errors)}");
                    Console.WriteLine($"ModelState keys: {string.Join(", ", ModelState.Keys)}");
                    
                    foreach (var key in ModelState.Keys)
                    {
                        var state = ModelState[key];
                        Console.WriteLine($"Key: {key}, ValidationState: {state.ValidationState}, Errors: {string.Join(", ", state.Errors.Select(e => e.ErrorMessage))}");
                    }
                    
                    return BadRequest(new { success = false, errors });
                }

                if (model.Document == null || model.Document.Length == 0)
                {
                    Console.WriteLine("No file provided");
                    return BadRequest(new { success = false, message = "No file provided" });
                }

                // Save file to /wwwroot/kyc/
                var kycFolder = Path.Combine(_env.WebRootPath, "kyc");
                Directory.CreateDirectory(kycFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Document.FileName);
                var filePath = Path.Combine(kycFolder, fileName);

                Console.WriteLine($"Saving file to: {filePath}");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Document.CopyToAsync(stream);
                }

                Console.WriteLine("File saved successfully");

                // Get user ID with logging
                var userId = User.GetUserId();
                Console.WriteLine($"Retrieved user ID: {userId}");

                // Send command to save KYC record
                var command = new UploadKYCCommand
                {
                    UserId = userId,
                    DocumentType = model.DocumentType,
                    FilePath = $"/kyc/{fileName}"
                };

                Console.WriteLine($"Sending command for user: {command.UserId}, DocumentType: {command.DocumentType}, FilePath: {command.FilePath}");
                
                try
                {
                    var result = await _mediator.Send(command);
                    Console.WriteLine($"KYC record saved successfully with ID: {result}");
                    return Ok(new { success = true, message = "KYC document uploaded successfully. Please wait for admin approval." });
                }
                catch (Exception mediatREx)
                {
                    Console.WriteLine($"MediatR command failed: {mediatREx.Message}");
                    Console.WriteLine($"MediatR stack trace: {mediatREx.StackTrace}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KYC Upload Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest(new { success = false, message = $"Upload failed: {ex.Message}" });
            }
        }
    }
}
