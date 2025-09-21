using AutoMapper;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingsByCustomer;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingById;
using CarRentalSystem.Application.Features.Customers.Queries.GetCustomerById;
using CarRentalSystem.Application.Features.Customers.Commands.UpdateCustomer;
using CarRentalSystem.Application.Contracts.Customer;
using CarRentalSystem.Web.ViewModels.Dashboard;
using CarRentalSystem.Web.ViewModels.Account;
using CarRentalSystem.Web.ViewModels.KYC;
using CarRentalSystem.Web.Extensions;
using CarRentalSystem.Application.Features.KYC.Command.UploadKYC;
using CarRentalSystem.Application.Features.KYC.Queries.HasUploadedKYC;
using CarRentalSystem.Application.Features.KYC.Queries.GetKYCByUserId;
using CarRentalSystem.Application.Features.Payments.Queries.GetPaymentsByCustomerId;
using CarRentalSystem.Infrastructure.Identity;
using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalSystem.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGoogleMapsService _googleMapsService;
        private readonly IPdfService _pdfService;

        public DashboardController(IMediator mediator, IMapper mapper, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, IGoogleMapsService googleMapsService, IPdfService pdfService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _googleMapsService = googleMapsService;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                // Check if user is admin - redirect to admin dashboard
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }

                // Check if user is a customer
                if (!User.IsInRole("Customer"))
                {
                    return Forbid("Access denied. Customer role required.");
                }

                var customer = await _mediator.Send(new GetCustomerByIdQuery(Guid.Parse(userId)));
                var bookings = await _mediator.Send(new GetBookingsByCustomerQuery(Guid.Parse(userId)));
                var payments = await _mediator.Send(new GetPaymentsByCustomerIdQuery(Guid.Parse(userId)));

                // Debug logging for payment data
                Console.WriteLine($"=== PAYMENT DEBUG ===");
                Console.WriteLine($"Customer ID: {userId}");
                Console.WriteLine($"Total bookings found: {bookings.Count}");
                Console.WriteLine($"Total payments found: {payments.Count}");
                
                // Check if there are any bookings with Paid status
                var paidBookings = bookings.Where(b => b.PaymentStatus == "Paid").ToList();
                Console.WriteLine($"Paid bookings count: {paidBookings.Count}");
                
                foreach (var booking in paidBookings)
                {
                    Console.WriteLine($"Paid Booking {booking.BookingId}: TotalCost={booking.TotalCost}, PaymentStatus={booking.PaymentStatus}");
                }
                
                foreach (var payment in payments)
                {
                    Console.WriteLine($"Payment {payment.PaymentId}: Amount={payment.Amount}, Method={payment.Method}, PaidAt={payment.PaidAt}");
                }

                // Calculate total spent from actual payment amounts
                var totalSpent = payments.Sum(p => p.Amount);
                Console.WriteLine($"Total spent calculated from payments: {totalSpent}");
                
                // Fallback: if no payments found, use booking data
                if (payments.Count == 0 && paidBookings.Count > 0)
                {
                    totalSpent = paidBookings.Sum(b => b.TotalCost);
                    Console.WriteLine($"No payments found, using booking data: {totalSpent}");
                }
                
                Console.WriteLine($"Final total spent: {totalSpent}");
                Console.WriteLine($"=== END PAYMENT DEBUG ===");
                
                var dashboardData = new UserDashboardViewModel
                {
                    Customer = customer,
                    RecentBookings = bookings.Take(5).ToList(),
                    TotalBookings = bookings.Count,
                    ActiveRentals = bookings.Count(b => b.BookingStatus == "Confirmed" || b.BookingStatus == "In Progress"),
                    TotalSpent = totalSpent,
                    Savings = 0 // Removed savings calculation
                };

                return View(dashboardData);
            }
            catch (KeyNotFoundException ex)
            {
                // Customer not found - redirect to profile setup
                TempData["Error"] = "Customer profile not found. Please complete your profile setup.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                // Log the exception and show a generic error
                TempData["Error"] = "An error occurred while loading your dashboard. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Route("Dashboard/Profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Check if user is admin - redirect to admin dashboard
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            // Check if user is a customer
            if (!User.IsInRole("Customer"))
            {
                return Forbid("Access denied. Customer role required.");
            }

            var customer = await _mediator.Send(new GetCustomerByIdQuery(Guid.Parse(userId)));
            return View(customer);
        }

        [HttpPost]
        [Route("Dashboard/UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(CustomerDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Check if user is admin - redirect to admin dashboard
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            // Check if user is a customer
            if (!User.IsInRole("Customer"))
            {
                return Forbid("Access denied. Customer role required.");
            }

            try
            {
                // Verify that the customer is updating their own profile
                if (model.Id != Guid.Parse(userId))
                {
                    TempData["Error"] = "You can only update your own profile.";
                    return RedirectToAction("Profile");
                }

                // Create update command
                var updateCommand = new UpdateCustomerCommand
                {
                    CustomerId = model.Id,
                    FullName = model.FullName,
                    Email = model.Email,
                    NIC = model.NIC,
                    Address = model.Address,
                    ProfileImagePath = model.ProfileImagePath
                };

                var result = await _mediator.Send(updateCommand);
                
                if (result)
                {
                    TempData["Success"] = "Profile updated successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to update profile. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating your profile. Please try again.";
                // Log the exception if needed
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        [Route("Dashboard/UploadProfilePicture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profileImage)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                if (profileImage == null || profileImage.Length == 0)
                {
                    TempData["Error"] = "Please select a valid image file.";
                    return RedirectToAction("Profile");
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["Error"] = "Please upload a valid image file (JPG, JPEG, PNG, or GIF).";
                    return RedirectToAction("Profile");
                }

                // Validate file size (max 5MB)
                if (profileImage.Length > 5 * 1024 * 1024)
                {
                    TempData["Error"] = "Image size must be less than 5MB.";
                    return RedirectToAction("Profile");
                }

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profile-pictures");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filename
                var fileName = $"{userId}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                // Update user profile image path in database
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    // Delete old profile picture if exists
                    if (!string.IsNullOrEmpty(user.ProfileImagePath))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfileImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    user.ProfileImagePath = $"/uploads/profile-pictures/{fileName}";
                    await _userManager.UpdateAsync(user);

                    // Update customer table as well
                    var customer = await _mediator.Send(new GetCustomerByIdQuery(Guid.Parse(userId)));
                    if (customer != null)
                    {
                        var updateCommand = new UpdateCustomerCommand
                        {
                            CustomerId = customer.Id,
                            FullName = customer.FullName,
                            Email = customer.Email,
                            NIC = customer.NIC,
                            Address = customer.Address,
                            ProfileImagePath = user.ProfileImagePath
                        };
                        await _mediator.Send(updateCommand);
                    }

                    TempData["Success"] = "Profile picture updated successfully!";
                }
                else
                {
                    TempData["Error"] = "User not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while uploading the profile picture. Please try again.";
                // Log the exception if needed
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        [Route("Dashboard/PasswordManagement")]
        public async Task<IActionResult> PasswordManagement()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Check if user has external logins (Google)
            var externalLogins = await _userManager.GetLoginsAsync(user);
            var isGoogleAccount = externalLogins.Any(login => login.LoginProvider == "Google");
            var hasPassword = await _userManager.HasPasswordAsync(user);

            var viewModel = new PasswordManagementViewModel
            {
                IsGoogleAccount = isGoogleAccount,
                HasPassword = hasPassword
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("Dashboard/SetPassword")]
        public async Task<IActionResult> SetPassword(PasswordManagementViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid)
            {
                return View("PasswordManagement", model);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("PasswordManagement");
                }

                // Check if user has external logins (Google)
                var externalLogins = await _userManager.GetLoginsAsync(user);
                var isGoogleAccount = externalLogins.Any(login => login.LoginProvider == "Google");
                var hasPassword = await _userManager.HasPasswordAsync(user);

                // For Google accounts, set initial password
                if (isGoogleAccount && !hasPassword)
                {
                    var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        // Verify the password was actually set
                        var passwordCheck = await _userManager.CheckPasswordAsync(user, model.NewPassword);
                        TempData["Success"] = $"Password set successfully! You can now login with email and password. Password verification: {(passwordCheck ? "SUCCESS" : "FAILED")}";
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    TempData["Error"] = $"Invalid operation. IsGoogleAccount: {isGoogleAccount}, HasPassword: {hasPassword}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while setting the password. Please try again.";
            }

            return View("PasswordManagement", model);
        }

        [HttpPost]
        [Route("Dashboard/ChangePassword")]
        public async Task<IActionResult> ChangePassword(PasswordManagementViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid)
            {
                return View("PasswordManagement", model);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("PasswordManagement");
                }

                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (!hasPassword)
                {
                    TempData["Error"] = "No password set. Please set a password first.";
                    return RedirectToAction("PasswordManagement");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Password changed successfully!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while changing the password. Please try again.";
            }

            return View("PasswordManagement", model);
        }

        [HttpGet]
        [Route("Dashboard/KYC")]
        public async Task<IActionResult> KYC()
        {
            var userId = User.GetUserId();
            var hasKyc = await _mediator.Send(new HasUploadedKYCQuery(userId));
            var kycUploads = await _mediator.Send(new GetKYCByUserIdQuery(userId));

            var viewModel = new KYCUploadViewModel();
            ViewBag.HasKyc = hasKyc;
            ViewBag.KycUploads = kycUploads;

            return View(viewModel);
        }

        [HttpPost]
        [Route("Dashboard/UploadKYC")]
        public async Task<IActionResult> UploadKYC(KYCUploadViewModel model)
        {
            var userId = User.GetUserId();
            
            if (!ModelState.IsValid)
            {
                var hasKyc = await _mediator.Send(new HasUploadedKYCQuery(userId));
                var kycUploads = await _mediator.Send(new GetKYCByUserIdQuery(userId));
                ViewBag.HasKyc = hasKyc;
                ViewBag.KycUploads = kycUploads;
                return View("KYC", model);
            }

            try
            {
                // Save file to /wwwroot/kyc/
                var kycFolder = Path.Combine(_webHostEnvironment.WebRootPath, "kyc");
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
                    UserId = userId,
                    DocumentType = model.DocumentType,
                    FilePath = $"/kyc/{fileName}"
                };

                await _mediator.Send(command);

                TempData["Success"] = "KYC document uploaded successfully. Please wait for admin approval.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while uploading the KYC document. Please try again.";
                var hasKyc = await _mediator.Send(new HasUploadedKYCQuery(userId));
                var kycUploads = await _mediator.Send(new GetKYCByUserIdQuery(userId));
                ViewBag.HasKyc = hasKyc;
                ViewBag.KycUploads = kycUploads;
                return View("KYC", model);
            }
        }

        [HttpGet]
        [Route("Dashboard/Bookings")]
        public async Task<IActionResult> Bookings(bool? paymentSuccess = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Check if user is admin - redirect to admin dashboard
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            // Check if user is a customer
            if (!User.IsInRole("Customer"))
            {
                return Forbid("Access denied. Customer role required.");
            }

            if (paymentSuccess == true)
            {
                TempData["Success"] = "Payment completed successfully! Your booking has been confirmed.";
            }

            var bookings = await _mediator.Send(new GetBookingsByCustomerQuery(Guid.Parse(userId)));
            return View(bookings);
        }

        [HttpGet]
        [Route("Dashboard/Bookings/{bookingId}")]
        public async Task<IActionResult> BookingDetails(Guid bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Check if user is admin - redirect to admin dashboard
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            // Check if user is a customer
            if (!User.IsInRole("Customer"))
            {
                return Forbid("Access denied. Customer role required.");
            }

            // Get the specific booking to verify ownership
            var booking = await _mediator.Send(new GetBookingByIdQuery { BookingId = bookingId });
            
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Bookings");
            }

            // Verify that the booking belongs to the current user
            if (booking.CustomerId != Guid.Parse(userId))
            {
                TempData["Error"] = "You can only view your own booking details.";
                return RedirectToAction("Bookings");
            }

            // Pass Google Maps API key to the view
            ViewBag.GoogleMapsApiKey = await _googleMapsService.GetApiKeyAsync();

            return View(booking);
        }

        [HttpGet]
        [Route("Dashboard/DownloadInvoice/{bookingId}")]
        public async Task<IActionResult> DownloadInvoice(Guid bookingId)
        {
            try
            {
                // Get booking details
                var booking = await _mediator.Send(new GetBookingByIdQuery { BookingId = bookingId });
                
                if (booking == null)
                {
                    TempData["Error"] = "Booking not found.";
                    return RedirectToAction("Bookings", "Dashboard");
                }

                // Verify that the booking belongs to the current user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null || booking.CustomerId != Guid.Parse(userId))
                {
                    TempData["Error"] = "You can only download invoices for your own bookings.";
                    return RedirectToAction("Bookings", "Dashboard");
                }

                // Generate invoice PDF
                var invoiceId = $"INV-{bookingId.ToString().Substring(0, 8)}";
                var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(booking, invoiceId);
                
                var fileName = $"Invoice_{bookingId.ToString().Substring(0, 8)}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating invoice.";
                return RedirectToAction("Bookings", "Dashboard");
            }
        }
    }
}
