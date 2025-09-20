using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Booking;
using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Application.Contracts.KYC;
using CarRentalSystem.Application.Contracts.Customer;
using CarRentalSystem.Application.Features.Cars.Commands.AdminApproveCar;
using CarRentalSystem.Application.Features.Cars.Commands.RegisterCar;
using CarRentalSystem.Application.Features.Cars.Commands.UpdateCar;
using CarRentalSystem.Application.Features.Cars.Commands.DeleteCar;
using CarRentalSystem.Application.Features.Cars.Queries.GetCarsByApprovalStatus;
using CarRentalSystem.Application.Features.Cars.Queries.GetAllCars;
using CarRentalSystem.Application.Features.KYC.Commands.AdminApproveOrRejectKYC;
using CarRentalSystem.Application.Features.KYC.Queries.GetAllKYCUploads;
using CarRentalSystem.Application.Features.KYC.Queries.GetAllKYCUploadsWithCustomerInfo;
using CarRentalSystem.Application.Features.KYC.Queries.GetKYCById;
using CarRentalSystem.Application.Features.Bookings.Queries.GetAllBookings;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingById;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingStatusIdByName;
using CarRentalSystem.Application.Features.Bookings.Queries.GetPaymentStatusIdByName;
using CarRentalSystem.Application.Features.Bookings.Commands.UpdateBookingStatus;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingKPIData;
using CarRentalSystem.Application.Features.Customers.Queries.GetCustomerKPIData;
using CarRentalSystem.Application.Features.Cars.Queries.GetCarKPIData;
using CarRentalSystem.Application.Features.Customers.Queries.GetAllCustomers;
using CarRentalSystem.Application.Features.Customers.Queries.GetCustomerById;
using CarRentalSystem.Application.Features.Customers.Commands.CreateCustomer;
using CarRentalSystem.Application.Features.Customers.Commands.UpdateCustomer;
using CarRentalSystem.Application.Features.Customers.Commands.DeleteCustomer;
using CarRentalSystem.Application.Features.Cars.Queries.GetCarById;
using CarRentalSystem.Application.Features.Cars.Queries.GetMyCars;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingsByCarOwner;
using CarRentalSystem.Application.Features.CarApproval.Queries.GetAllCarApprovalStatuses;
using CarRentalSystem.Web.ViewModels.Admin;
using CarRentalSystem.Infrastructure.Identity;
using CarRentalSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Helpers.EmailTemplates;

namespace CarRentalSystem.Web.Controllers
{
    [Route("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ICarRepository _carRepository;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public AdminController(IMediator mediator, IMapper mapper, ICarRepository carRepository, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _carRepository = carRepository;
            _env = env;
            _userManager = userManager;
            _emailService = emailService;
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            // Redirect to the proper admin login
            return RedirectToAction("AdminLogin", "Account");
        }


        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var pendingCars = await _mediator.Send(new GetCarsByApprovalStatusQuery("Pending"));
                var approvedCars = await _mediator.Send(new GetCarsByApprovalStatusQuery("Approved"));
                var rejectedCars = await _mediator.Send(new GetCarsByApprovalStatusQuery("Rejected"));
                var kycUploads = await _mediator.Send(new GetAllKYCUploadsQuery());
                var bookings = await _mediator.Send(new GetAllBookingsQuery());
                var customers = await _mediator.Send(new GetAllCustomersQuery());

                // Calculate real metrics from database
                var totalRevenue = bookings.Where(b => b.PaymentStatus != null && b.PaymentStatus.Name == "Paid").Sum(b => b.TotalCost);
                var pendingRevenue = bookings.Where(b => b.PaymentStatus != null && b.PaymentStatus.Name == "Pending").Sum(b => b.TotalCost);
                var totalBookings = bookings.Count;
                var confirmedBookings = bookings.Count(b => b.BookingStatus != null && b.BookingStatus.Name == "Confirmed");
                var activeCustomers = bookings.Select(b => b.CustomerId).Distinct().Count();

                var dashboardData = new AdminDashboardViewModel
                {
                    PendingCars = _mapper.Map<List<CarDto>>(pendingCars),
                    PendingKYC = _mapper.Map<List<KYCUploadDto>>(kycUploads),
                    RecentBookings = _mapper.Map<List<BookingDto>>(bookings.Take(10).ToList()), // Show only recent 10
                    TotalCustomers = customers.Count,
                    ApprovedCars = approvedCars.Count(),
                    RejectedCars = rejectedCars.Count(),
                    TotalRevenue = totalRevenue
                };

                // Set ViewBag for sidebar notifications and additional metrics
                ViewBag.PendingCarsCount = pendingCars.Count;
                ViewBag.PendingKYCCount = kycUploads.Count(c => c.Status == "Pending");
                ViewBag.TotalRevenue = totalRevenue;
                ViewBag.PendingRevenue = pendingRevenue;
                ViewBag.TotalBookings = totalBookings;
                ViewBag.ConfirmedBookings = confirmedBookings;
                ViewBag.ActiveCustomers = activeCustomers;

                Console.WriteLine($"Dashboard loaded - Revenue: ${totalRevenue:N2}, Bookings: {totalBookings}, Customers: {customers.Count}");

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading admin dashboard: {ex.Message}");
                TempData["Error"] = "Error loading dashboard data: " + ex.Message;
                
                // Return empty dashboard on error
                return View(new AdminDashboardViewModel());
            }
        }




        [HttpGet]
        [Route("Admin/KYC")]
        public async Task<IActionResult> KYC()
        {
            var kycUploads = await _mediator.Send(new GetAllKYCUploadsWithCustomerInfoQuery());
            return View(kycUploads);
        }

        [HttpPost]
        [Route("Admin/KYC/Approve/{kycId}")]
        public async Task<IActionResult> ApproveKYC(Guid kycId)
        {
            try
            {
                // Log the approval attempt
                Console.WriteLine($"AdminController: Attempting to approve KYC: {kycId}");
                Console.WriteLine($"AdminController: Request method: {Request.Method}");
                Console.WriteLine($"AdminController: Request path: {Request.Path}");
                Console.WriteLine($"AdminController: Anti-forgery token present: {Request.Headers.ContainsKey("RequestVerificationToken")}");
                
                var command = new AdminApproveOrRejectKYCCommand 
                { 
                    KYCId = kycId, 
                    IsApproved = true,
                    NewStatus = "Approved"
                };
                
                Console.WriteLine($"AdminController: Sending command with KYCId: {command.KYCId}, IsApproved: {command.IsApproved}, NewStatus: {command.NewStatus}");
                
                var result = await _mediator.Send(command);
                Console.WriteLine($"AdminController: KYC approval result: {result}");

                if (result)
                {
                    TempData["Success"] = "KYC approved successfully!";
                    Console.WriteLine($"AdminController: KYC {kycId} approved successfully");
                }
                else
                {
                    TempData["Error"] = "Failed to approve KYC. It may already be processed.";
                    Console.WriteLine($"AdminController: KYC {kycId} approval failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AdminController: Error approving KYC: {ex.Message}");
                Console.WriteLine($"AdminController: Stack trace: {ex.StackTrace}");
                TempData["Error"] = "Error approving KYC: " + ex.Message;
            }

            return RedirectToAction("KYC");
        }

        [HttpPost]
        [Route("Admin/KYC/Reject/{kycId}")]
        public async Task<IActionResult> RejectKYC(Guid kycId, string? remarks = null)
        {
            try
            {
                // Log the rejection attempt
                Console.WriteLine($"Attempting to reject KYC: {kycId}");
                
                var command = new AdminApproveOrRejectKYCCommand 
                { 
                    KYCId = kycId, 
                    IsApproved = false,
                    NewStatus = "Rejected",
                    Remarks = remarks
                };
                
                var result = await _mediator.Send(command);
                Console.WriteLine($"KYC rejection result: {result}");

                if (result)
                {
                    TempData["Success"] = "KYC rejected successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to reject KYC. It may already be processed.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rejecting KYC: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = "Error rejecting KYC: " + ex.Message;
            }

            return RedirectToAction("KYC");
        }

        [HttpGet]
        [Route("Admin/KYC/View/{kycId}")]
        public async Task<IActionResult> ViewKYCDocument(Guid kycId)
        {
            try
            {
                var kyc = await _mediator.Send(new GetKYCByIdQuery(kycId));
                if (kyc == null)
                {
                    TempData["Error"] = "KYC document not found.";
                    return RedirectToAction("KYC");
                }

                // Check if file exists
                var filePath = Path.Combine(_env.WebRootPath, kyc.FilePath.TrimStart('/'));
                if (!System.IO.File.Exists(filePath))
                {
                    TempData["Error"] = "Document file not found on server.";
                    return RedirectToAction("KYC");
                }

                // Return the file
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(filePath);
                var fileName = Path.GetFileName(filePath);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error viewing document: " + ex.Message;
                return RedirectToAction("KYC");
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".tiff" => "image/tiff",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }

        [HttpGet]
        [Route("Bookings")]
        public async Task<IActionResult> Bookings()
        {
            var bookings = await _mediator.Send(new GetAllBookingsQuery());
            return View(_mapper.Map<List<BookingDto>>(bookings));
        }

        [HttpGet]
        [Route("Customers")]
        public async Task<IActionResult> Customers()
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery());
            
            // Remove duplicates based on email (keep the first occurrence)
            var uniqueCustomers = customers
                .GroupBy(c => c.Email)
                .Select(g => g.First())
                .ToList();
            
            Console.WriteLine($"Customers: Total={customers.Count}, Unique={uniqueCustomers.Count}");
            
            return View(uniqueCustomers);
        }

        [HttpGet]
        [Route("Customers/Create")]
        public IActionResult CreateCustomer()
        {
            var model = new CreateCustomerViewModel();
            return View(model);
        }

        [HttpPost]
        [Route("Customers/Create")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Handle profile image upload
                string? profileImagePath = null;
                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    // Create uploads directory if it doesn't exist
                    var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "profile-pictures");
                    Directory.CreateDirectory(uploadsPath);

                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}_{model.ProfileImage.FileName}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImage.CopyToAsync(stream);
                    }

                    profileImagePath = fileName; // Store only filename in database
                    Console.WriteLine($"Profile image saved: {fileName} to {filePath}");
                }

                // Generate password
                string password;
                if (model.GenerateRandomPassword)
                {
                    password = GenerateRandomPassword();
                }
                else
                {
                    password = model.CustomPassword ?? GenerateRandomPassword();
                }

                // Create Identity user
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    NICNumber = model.NIC,
                    Address = model.Address,
                    ProfileImagePath = profileImagePath,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Add to Customer role
                    await _userManager.AddToRoleAsync(user, "Customer");

                    // Create customer record in database using the repository directly
                    var customer = new Customer
                    {
                        Id = user.Id,
                        FullName = model.FullName,
                        Email = model.Email,
                        NIC = model.NIC,
                        Address = model.Address,
                        ProfileImagePath = profileImagePath
                    };

                    // Use the customer repository directly instead of the command to avoid duplicates
                    var customerRepository = HttpContext.RequestServices.GetRequiredService<ICustomerRepository>();
                    var customerId = await customerRepository.CreateCustomerAsync(customer, CancellationToken.None);

                    // Send email with credentials if requested
                    if (model.SendCredentialsEmail)
                    {
                        await SendCredentialsEmail(model.Email, model.FullName, password);
                    }

                    TempData["Success"] = $"Customer created successfully! {(model.SendCredentialsEmail ? "Login credentials have been sent to the customer's email." : "")}";
                    return RedirectToAction("Customers");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["Error"] = $"Error creating customer: {errors}";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating customer: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = "Error creating customer: " + ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        [Route("Customers/Edit/{customerId}")]
        public async Task<IActionResult> EditCustomer(Guid customerId)
        {
            try
            {
                var customer = await _mediator.Send(new GetCustomerByIdQuery(customerId));
                if (customer == null)
                {
                    TempData["Error"] = "Customer not found.";
                    return RedirectToAction("Customers");
                }

                var command = new UpdateCustomerCommand
                {
                    CustomerId = customer.Id,
                    FullName = customer.FullName,
                    Email = customer.Email,
                    ProfileImagePath = customer.ProfileImagePath,
                    NIC = customer.NIC,
                    Address = customer.Address
                };

                return View(command);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading customer: " + ex.Message;
                return RedirectToAction("Customers");
            }
        }

        [HttpPost]
        [Route("Customers/Edit/{customerId}")]
        public async Task<IActionResult> EditCustomer(Guid customerId, UpdateCustomerCommand command)
        {
            if (customerId != command.CustomerId)
            {
                TempData["Error"] = "Customer ID mismatch.";
                return RedirectToAction("Customers");
            }

            if (!ModelState.IsValid)
            {
                return View(command);
            }

            try
            {
                var result = await _mediator.Send(command);
                if (result)
                {
                    TempData["Success"] = "Customer updated successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to update customer.";
                }
                return RedirectToAction("Customers");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating customer: " + ex.Message;
                return View(command);
            }
        }

        [HttpPost]
        [Route("Customers/Delete/{customerId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomer(Guid customerId)
        {
            try
            {
                Console.WriteLine($"DeleteCustomer: Action method called for customer {customerId}");
                Console.WriteLine($"DeleteCustomer: Request method: {Request.Method}");
                Console.WriteLine($"DeleteCustomer: Request path: {Request.Path}");
                Console.WriteLine($"DeleteCustomer: Request query: {Request.QueryString}");
                
                // First, get the customer details to handle profile image deletion
                var customer = await _mediator.Send(new GetCustomerByIdQuery(customerId));
                if (customer != null && !string.IsNullOrEmpty(customer.ProfileImagePath))
                {
                    // Delete the profile image file
                    try
                    {
                        var imagePath = Path.Combine(_env.WebRootPath, "uploads", "profile-pictures", customer.ProfileImagePath);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                            Console.WriteLine($"DeleteCustomer: Deleted profile image {customer.ProfileImagePath}");
                        }
                    }
                    catch (Exception imgEx)
                    {
                        Console.WriteLine($"DeleteCustomer: Error deleting profile image: {imgEx.Message}");
                        // Continue with deletion even if image deletion fails
                    }
                }
                
                // Try to find and delete the Identity user
                var user = await _userManager.FindByIdAsync(customerId.ToString());
                if (user != null)
                {
                    Console.WriteLine($"DeleteCustomer: Found Identity user {user.Email}");
                    
                    // Delete the Identity user (this will also handle related data)
                    var identityResult = await _userManager.DeleteAsync(user);
                    if (identityResult.Succeeded)
                    {
                        Console.WriteLine($"DeleteCustomer: Successfully deleted Identity user {user.Email}");
                    }
                    else
                    {
                        Console.WriteLine($"DeleteCustomer: Failed to delete Identity user: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                        // Continue with customer deletion even if Identity deletion fails
                    }
                }
                else
                {
                    Console.WriteLine($"DeleteCustomer: No Identity user found for {customerId}");
                }
                
                // Delete the customer record from the database
                var command = new DeleteCustomerCommand { CustomerId = customerId };
                var result = await _mediator.Send(command);
                
                if (result)
                {
                    Console.WriteLine($"DeleteCustomer: Successfully deleted customer record {customerId}");
                    TempData["Success"] = "Customer deleted successfully!";
                }
                else
                {
                    Console.WriteLine($"DeleteCustomer: Failed to delete customer record {customerId}");
                    TempData["Error"] = "Failed to delete customer record.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteCustomer: Error deleting customer {customerId}: {ex.Message}");
                Console.WriteLine($"DeleteCustomer: Stack trace: {ex.StackTrace}");
                TempData["Error"] = "Error deleting customer: " + ex.Message;
            }

            return RedirectToAction("Customers");
        }


        #region Car Owner Management

        [HttpGet]
        [Route("CarOwners")]
        public async Task<IActionResult> CarOwners()
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery());
            var allCars = await _mediator.Send(new GetAllCarsQuery());
            var allBookings = await _mediator.Send(new GetAllBookingsQuery());

            var carOwners = new List<CarOwnerDetailsDto>();

            foreach (var customer in customers)
            {
                var ownerCars = allCars.Where(c => c.OwnerId == customer.Id).ToList();
                var ownerBookings = allBookings.Where(b => ownerCars.Any(c => c.CarId == b.CarId)).ToList();

                var ownerDetails = new CarOwnerDetailsDto
                {
                    CustomerId = customer.Id,
                    FullName = customer.FullName,
                    Email = customer.Email,
                    ProfileImagePath = customer.ProfileImagePath,
                    NIC = customer.NIC,
                    Address = customer.Address,
                    CreatedDate = DateTime.Now, // CustomerDto doesn't have CreatedDate
                    TotalCars = ownerCars.Count,
                    ActiveCars = ownerCars.Count(c => c.ApprovalStatus == "Approved"),
                    PendingCars = ownerCars.Count(c => c.ApprovalStatus == "Pending"),
                    RejectedCars = ownerCars.Count(c => c.ApprovalStatus == "Rejected"),
                    TotalBookings = ownerBookings.Count,
                    ActiveBookings = ownerBookings.Count(b => b.BookingStatus != null && b.BookingStatus.Name == "Confirmed"),
                    TotalEarnings = ownerBookings.Where(b => b.PaymentStatus != null && b.PaymentStatus.Name == "Paid").Sum(b => b.TotalCost),
                    MonthlyEarnings = ownerBookings.Where(b => b.PaymentStatus != null && b.PaymentStatus.Name == "Paid" && b.CreatedAt.Month == DateTime.Now.Month).Sum(b => b.TotalCost),
                    Cars = _mapper.Map<List<CarDto>>(ownerCars.Take(5)), // Show only first 5 cars
                    RecentBookings = _mapper.Map<List<BookingDto>>(ownerBookings.Take(5)) // Show only first 5 bookings
                };

                carOwners.Add(ownerDetails);
            }

            var model = new CarOwnerManagementViewModel
            {
                CarOwners = carOwners,
                TotalOwners = carOwners.Count,
                ActiveOwners = carOwners.Count(o => o.TotalCars > 0),
                InactiveOwners = carOwners.Count(o => o.TotalCars == 0),
                TotalEarnings = carOwners.Sum(o => o.TotalEarnings)
            };

            return View(model);
        }

        [HttpGet]
        [Route("CarOwners/Details/{customerId}")]
        public async Task<IActionResult> GetCarOwnerDetails(Guid customerId)
        {
            try
            {
                var customer = await _mediator.Send(new GetCustomerByIdQuery(customerId));
                if (customer == null)
                {
                    return NotFound();
                }

                var ownerCars = await _mediator.Send(new GetMyCarsQuery(customerId));
                var ownerBookings = await _mediator.Send(new GetBookingsByCarOwnerQuery(customerId));

                var html = $@"
                    <div class='row'>
                        <div class='col-md-4'>
                            <div class='text-center'>
                                {(string.IsNullOrEmpty(customer.ProfileImagePath) ? 
                                    "<div class='profile-placeholder-large'><i class='bi bi-person'></i></div>" : 
                                    $"<img src='{customer.ProfileImagePath}' alt='{customer.FullName}' class='img-fluid rounded-circle mb-3' style='width: 150px; height: 150px; object-fit: cover;'>")}
                                <h5>{customer.FullName}</h5>
                                <p class='text-muted'>{customer.Email}</p>
                            </div>
                        </div>
                        <div class='col-md-8'>
                            <h6>Profile Information</h6>
                            <p><strong>NIC:</strong> {customer.NIC ?? "Not provided"}</p>
                            <p><strong>Address:</strong> {customer.Address ?? "Not provided"}</p>
                                            <p><strong>Member Since:</strong> {DateTime.Now:MMM dd, yyyy}</p>
                            <hr>
                            <h6>Statistics</h6>
                            <div class='row'>
                                <div class='col-6'>
                                    <p><strong>Total Cars:</strong> {ownerCars.Count}</p>
                                    <p><strong>Active Cars:</strong> {ownerCars.Count(c => c.ApprovalStatus == "Approved")}</p>
                                </div>
                                <div class='col-6'>
                                    <p><strong>Total Bookings:</strong> {ownerBookings.Count}</p>
                                    <p><strong>Total Earnings:</strong> ${ownerBookings.Where(b => b.PaymentStatus != null && b.PaymentStatus.Name == "Paid").Sum(b => b.TotalCost):F2}</p>
                                </div>
                            </div>
                        </div>
                    </div>";

                return Json(new { html });
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error loading car owner details" });
            }
        }

        #endregion

        #region Enhanced Booking Management

        [HttpGet]
        [Route("Bookings/Enhanced")]
        public async Task<IActionResult> BookingsEnhanced(string? status, DateTime? startDate, DateTime? endDate)
        {
            var allBookings = await _mediator.Send(new GetAllBookingsQuery());
            var allCars = await _mediator.Send(new GetAllCarsQuery());
            var allCustomers = await _mediator.Send(new GetAllCustomersQuery());

            var bookingDetails = allBookings.Select(booking =>
            {
                var car = allCars.FirstOrDefault(c => c.CarId == booking.CarId);
                var customer = allCustomers.FirstOrDefault(c => c.Id == booking.CustomerId);
                var carOwner = allCustomers.FirstOrDefault(c => c.Id == car?.OwnerId);

                // Debug logging
                Console.WriteLine($"Booking {booking.BookingId}:");
                Console.WriteLine($"  - Car: {car?.Name} {car?.Model} (ImagePath: '{car?.ImagePath}')");
                Console.WriteLine($"  - Customer: {customer?.FullName} ({customer?.Email})");
                Console.WriteLine($"  - Payment Status: {booking.PaymentStatus?.Name}");
                Console.WriteLine($"  - Booking Status: {booking.BookingStatus?.Name}");

                return new BookingDetailsDto
                {
                    BookingId = booking.BookingId,
                    StartDate = booking.PickupDate,
                    EndDate = booking.ReturnDate,
                    TotalAmount = booking.TotalCost,
                    Status = booking.BookingStatus?.Name ?? "Unknown",
                    PaymentStatus = booking.PaymentStatus?.Name ?? "Unknown",
                    CreatedDate = booking.CreatedAt,
                    UpdatedDate = booking.CreatedAt, // Use CreatedAt as UpdatedDate is not available
                    Notes = "", // Notes property is not available in Booking entity
                    CarId = car?.CarId ?? Guid.Empty,
                    CarName = car?.Name ?? "Unknown",
                    CarModel = car?.Model ?? "Unknown",
                    CarImagePath = car?.ImagePath,
                    CarRatePerDay = car?.RatePerDay ?? 0,
                    CustomerId = customer?.Id ?? Guid.Empty,
                    CustomerName = customer?.FullName ?? "Unknown",
                    CustomerEmail = customer?.Email ?? "Unknown",
                    CarOwnerId = carOwner?.Id ?? Guid.Empty,
                    CarOwnerName = carOwner?.FullName ?? "Unknown",
                    CarOwnerEmail = carOwner?.Email ?? "Unknown"
                };
            }).ToList();

            // Apply filters
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                bookingDetails = bookingDetails.Where(b => b.Status == status).ToList();
            }

            if (startDate.HasValue)
            {
                bookingDetails = bookingDetails.Where(b => b.StartDate >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                bookingDetails = bookingDetails.Where(b => b.EndDate <= endDate.Value).ToList();
            }

            var model = new BookingManagementViewModel
            {
                Bookings = bookingDetails,
                TotalBookings = bookingDetails.Count,
                PendingBookings = bookingDetails.Count(b => b.Status == "Pending"),
                ConfirmedBookings = bookingDetails.Count(b => b.Status == "Confirmed"),
                CompletedBookings = bookingDetails.Count(b => b.Status == "Completed"),
                CancelledBookings = bookingDetails.Count(b => b.Status == "Cancelled"),
                TotalRevenue = bookingDetails.Where(b => b.PaymentStatus == "Paid").Sum(b => b.TotalAmount),
                MonthlyRevenue = bookingDetails.Where(b => b.PaymentStatus == "Paid" && b.CreatedDate.Month == DateTime.Now.Month).Sum(b => b.TotalAmount),
                FilterStatus = status,
                FilterStartDate = startDate,
                FilterEndDate = endDate
            };

            return View(model);
        }

        [HttpGet]
        [Route("Bookings/Export")]
        public async Task<IActionResult> ExportBookings(string? status, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Get all bookings data (same logic as BookingsEnhanced)
                var allBookings = await _mediator.Send(new GetAllBookingsQuery());
                var allCars = await _mediator.Send(new GetAllCarsQuery());
                var allCustomers = await _mediator.Send(new GetAllCustomersQuery());

                var bookingDetails = allBookings.Select(booking =>
                {
                    var car = allCars.FirstOrDefault(c => c.CarId == booking.CarId);
                    var customer = allCustomers.FirstOrDefault(c => c.Id == booking.CustomerId);
                    var carOwner = allCustomers.FirstOrDefault(c => c.Id == car?.OwnerId);

                    return new BookingDetailsDto
                    {
                        BookingId = booking.BookingId,
                        StartDate = booking.PickupDate,
                        EndDate = booking.ReturnDate,
                        TotalAmount = booking.TotalCost,
                        Status = booking.BookingStatus?.Name ?? "Unknown",
                        PaymentStatus = booking.PaymentStatus?.Name ?? "Unknown",
                        CreatedDate = booking.CreatedAt,
                        UpdatedDate = booking.CreatedAt,
                        Notes = "",
                        CarId = car?.CarId ?? Guid.Empty,
                        CarName = car?.Name ?? "Unknown",
                        CarModel = car?.Model ?? "Unknown",
                        CarImagePath = car?.ImagePath,
                        CarRatePerDay = car?.RatePerDay ?? 0,
                        CustomerId = customer?.Id ?? Guid.Empty,
                        CustomerName = customer?.FullName ?? "Unknown",
                        CustomerEmail = customer?.Email ?? "Unknown",
                        CarOwnerId = carOwner?.Id ?? Guid.Empty,
                        CarOwnerName = carOwner?.FullName ?? "Unknown",
                        CarOwnerEmail = carOwner?.Email ?? "Unknown"
                    };
                }).ToList();

                // Apply filters (same as BookingsEnhanced)
                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    bookingDetails = bookingDetails.Where(b => b.Status == status).ToList();
                }

                if (startDate.HasValue)
                {
                    bookingDetails = bookingDetails.Where(b => b.StartDate >= startDate.Value).ToList();
                }

                if (endDate.HasValue)
                {
                    bookingDetails = bookingDetails.Where(b => b.EndDate <= endDate.Value).ToList();
                }

                // Create Excel file
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Bookings");

                // Add headers
                var headers = new[]
                {
                    "Booking ID", "Customer Name", "Customer Email", "Customer Phone",
                    "Car Name", "Car Model", "Car Owner", "Car Owner Email",
                    "Start Date", "End Date", "Duration (Days)", "Daily Rate",
                    "Total Amount", "Booking Status", "Payment Status",
                    "Created Date", "Notes"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, headers.Length])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Add data
                for (int i = 0; i < bookingDetails.Count; i++)
                {
                    var booking = bookingDetails[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = booking.BookingId.ToString();
                    worksheet.Cells[row, 2].Value = booking.CustomerName;
                    worksheet.Cells[row, 3].Value = booking.CustomerEmail;
                    worksheet.Cells[row, 4].Value = ""; // Customer phone not available
                    worksheet.Cells[row, 5].Value = booking.CarName;
                    worksheet.Cells[row, 6].Value = booking.CarModel;
                    worksheet.Cells[row, 7].Value = booking.CarOwnerName;
                    worksheet.Cells[row, 8].Value = booking.CarOwnerEmail;
                    worksheet.Cells[row, 9].Value = booking.StartDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 10].Value = booking.EndDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 11].Value = (booking.EndDate - booking.StartDate).Days;
                    worksheet.Cells[row, 12].Value = booking.CarRatePerDay;
                    worksheet.Cells[row, 13].Value = booking.TotalAmount;
                    worksheet.Cells[row, 14].Value = booking.Status;
                    worksheet.Cells[row, 15].Value = booking.PaymentStatus;
                    worksheet.Cells[row, 16].Value = booking.CreatedDate.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[row, 17].Value = booking.Notes;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Add borders to all data cells
                using (var range = worksheet.Cells[1, 1, bookingDetails.Count + 1, headers.Length])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                // Generate filename with timestamp
                var fileName = $"Bookings_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
                // Convert to byte array
                var fileBytes = package.GetAsByteArray();

                // Return file
                return File(fileBytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting bookings: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = "Error exporting bookings: " + ex.Message;
                return RedirectToAction("BookingsEnhanced");
            }
        }

        [HttpPost]
        [Route("Bookings/UpdateStatus/{bookingId}")]
        public async Task<IActionResult> UpdateBookingStatus(Guid bookingId, string status)
        {
            try
            {
                Console.WriteLine($"UpdateBookingStatus: Updating booking {bookingId} to status '{status}'");
                
                var bookingStatusId = await GetBookingStatusId(status);
                
                var command = new UpdateBookingStatusCommand
                {
                    BookingId = bookingId,
                    BookingStatusId = bookingStatusId
                };

                var result = await _mediator.Send(command);
                Console.WriteLine($"UpdateBookingStatus: Result = {result}");
                
                if (result)
                {
                    TempData["Success"] = $"Booking status updated to {status} successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to update booking status.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating booking status: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = $"An error occurred while updating the booking status: {ex.Message}";
            }

            return RedirectToAction("BookingsEnhanced");
        }

        [HttpGet]
        [Route("Bookings/Details/{bookingId}")]
        public async Task<IActionResult> GetBookingDetails(Guid bookingId)
        {
            try
            {
                Console.WriteLine($"GetBookingDetails: Loading details for booking {bookingId}");
                
                var booking = await _mediator.Send(new GetBookingByIdQuery { BookingId = bookingId });
                if (booking == null)
                {
                    Console.WriteLine($"GetBookingDetails: Booking {bookingId} not found");
                    return Json(new { error = "Booking not found" });
                }

                Console.WriteLine($"GetBookingDetails: Found booking - Status: {booking.BookingStatus}, Payment: {booking.PaymentStatus}");

                var html = $@"
                    <div class='row'>
                        <div class='col-md-6'>
                            <h6>Booking Information</h6>
                            <p><strong>Booking ID:</strong> #{booking.BookingId.ToString().Substring(0, 8)}</p>
                            <p><strong>Status:</strong> <span class='badge bg-primary'>{booking.BookingStatus}</span></p>
                            <p><strong>Payment Status:</strong> <span class='badge bg-success'>{booking.PaymentStatus}</span></p>
                            <p><strong>Pickup Date:</strong> {booking.PickupDate:MMM dd, yyyy}</p>
                            <p><strong>Return Date:</strong> {booking.ReturnDate:MMM dd, yyyy}</p>
                            <p><strong>Total Cost:</strong> ${booking.TotalCost:F2}</p>
                            <p><strong>Created:</strong> {DateTime.Now:MMM dd, yyyy HH:mm}</p>
                        </div>
                        <div class='col-md-6'>
                            <h6>Car Information</h6>
                            <p><strong>Car:</strong> {booking.CarName}</p>
                            <p><strong>Pickup Location:</strong> {booking.PickupLocation}</p>
                            <p><strong>Customer:</strong> {booking.CustomerName}</p>
                        </div>
                    </div>";

                return Json(new { html });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetBookingDetails: Error loading booking {bookingId}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { error = $"Error loading booking details: {ex.Message}" });
            }
        }

        private async Task<Guid> GetBookingStatusId(string status)
        {
            try
            {
                // Get the actual status ID from the database
                var statusId = await _mediator.Send(new GetBookingStatusIdByNameQuery(status));
                Console.WriteLine($"GetBookingStatusId: Found status '{status}' with ID: {statusId}");
                return statusId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting booking status ID for '{status}': {ex.Message}");
                throw new Exception($"Booking status '{status}' not found in database.");
            }
        }

        #endregion


        #region Database Maintenance

        [HttpPost]
        [Route("Maintenance/AddMissingStatuses")]
        public async Task<IActionResult> AddMissingBookingStatuses()
        {
            try
            {
                Console.WriteLine("Adding missing booking statuses...");
                
                // This is a simple way to add missing statuses without complex seeding
                // In production, you'd want to use proper migrations
                TempData["Success"] = "Missing booking statuses will be added on next application restart. Please restart the application.";
                
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding missing statuses: {ex.Message}");
                TempData["Error"] = $"Error adding missing statuses: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Route("Bookings/UpdatePaymentStatus/{bookingId}")]
        public async Task<IActionResult> UpdatePaymentStatus(Guid bookingId, string paymentStatus)
        {
            try
            {
                Console.WriteLine($"UpdatePaymentStatus: Updating booking {bookingId} payment status to '{paymentStatus}'");
                
                var paymentStatusId = await _mediator.Send(new GetPaymentStatusIdByNameQuery(paymentStatus));
                
                var command = new UpdateBookingStatusCommand
                {
                    BookingId = bookingId,
                    BookingStatusId = Guid.Empty, // Don't change booking status
                    PaymentStatusId = paymentStatusId
                };

                var result = await _mediator.Send(command);
                Console.WriteLine($"UpdatePaymentStatus: Result = {result}");
                
                if (result)
                {
                    TempData["Success"] = $"Payment status updated to {paymentStatus} successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to update payment status.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating payment status: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = $"Error updating payment status: {ex.Message}";
            }

            return RedirectToAction("BookingsEnhanced");
        }

        #endregion

        #region KPI Dashboard

        [HttpGet]
        [Route("KPI")]
        public async Task<IActionResult> KPI()
        {
            try
            {
                var bookingKPIData = await _mediator.Send(new GetBookingKPIDataQuery());
                var customerKPIData = await _mediator.Send(new GetCustomerKPIDataQuery());
                var carKPIData = await _mediator.Send(new GetCarKPIDataQuery());

                var kpiViewModel = new KPIViewModel
                {
                    BookingKPIData = bookingKPIData,
                    CustomerKPIData = customerKPIData,
                    CarKPIData = carKPIData
                };

                return View(kpiViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading KPI data: {ex.Message}");
                TempData["Error"] = "Error loading KPI data: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Route("KPI/BookingData")]
        public async Task<IActionResult> GetBookingKPIData(int? year, int? month)
        {
            try
            {
                var bookingKPIData = await _mediator.Send(new GetBookingKPIDataQuery(year, month));
                return Json(bookingKPIData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading booking KPI data: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("KPI/CustomerData")]
        public async Task<IActionResult> GetCustomerKPIData()
        {
            try
            {
                var customerKPIData = await _mediator.Send(new GetCustomerKPIDataQuery());
                return Json(customerKPIData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading customer KPI data: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("KPI/CarData")]
        public async Task<IActionResult> GetCarKPIData()
        {
            try
            {
                var carKPIData = await _mediator.Send(new GetCarKPIDataQuery());
                return Json(carKPIData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading car KPI data: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        #endregion

        #region Helper Methods

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var chars = new char[length];
            
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }
            
            return new string(chars);
        }

        private async Task SendCredentialsEmail(string email, string fullName, string password)
        {
            try
            {
                // Generate HTML email content using the template
                var htmlBody = CustomerCredentialsEmailTemplateHelper.GenerateHtml(fullName, email, password);
                
                // Send email using the email service
                await _emailService.SendEmailAsync(
                    email,
                    "Welcome to CarRentalSystem - Your Account Credentials",
                    htmlBody
                );
                
                Console.WriteLine($"Credentials email sent successfully to: {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending credentials email to {email}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Don't throw exception as this shouldn't prevent customer creation
            }
        }

        #endregion
    }
}
