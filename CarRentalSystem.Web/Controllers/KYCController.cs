using CarRentalSystem.Application.Features.KYC.Command.UploadKYC;
using CarRentalSystem.Web.Extensions;
using CarRentalSystem.Web.ViewModels.KYC;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Web.Controllers
{
    [Authorize]
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
    }
}
