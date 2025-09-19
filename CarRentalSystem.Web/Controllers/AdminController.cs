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
using CarRentalSystem.Application.Features.Cars.Queries.GetCarById;
using CarRentalSystem.Application.Features.Cars.Queries.GetMyCars;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingsByCarOwner;
using CarRentalSystem.Web.ViewModels.Admin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

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

        public AdminController(IMediator mediator, IMapper mapper, ICarRepository carRepository, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _mapper = mapper;
            _carRepository = carRepository;
            _env = env;
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

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            // Use Identity logout
            await HttpContext.SignOutAsync();
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
        [Route("Cars")]
        public async Task<IActionResult> Cars()
        {
            // Check if user is admin
            if (!IsAdmin())
            {
                return RedirectToAction("Login");
            }

            var pendingCars = await _carRepository.GetCarsByApprovalStatusWithOwnerAsync("Pending", CancellationToken.None);
            var approvedCars = await _carRepository.GetCarsByApprovalStatusWithOwnerAsync("Approved", CancellationToken.None);
            var rejectedCars = await _carRepository.GetCarsByApprovalStatusWithOwnerAsync("Rejected", CancellationToken.None);

            // Debug: Log car data
            Console.WriteLine($"Cars Action - Pending: {pendingCars.Count}, Approved: {approvedCars.Count}, Rejected: {rejectedCars.Count}");
            foreach (var car in pendingCars.Take(3))
            {
                Console.WriteLine($"Pending Car: {car.Name} {car.Model}, ImagePath: '{car.ImagePath}', Owner: {car.OwnerName}");
            }
            foreach (var car in approvedCars.Take(3))
            {
                Console.WriteLine($"Approved Car: {car.Name} {car.Model}, ImagePath: '{car.ImagePath}', Owner: {car.OwnerName}");
            }

            var model = new CarManagementViewModel
            {
                PendingCars = pendingCars,
                ApprovedCars = approvedCars,
                RejectedCars = rejectedCars
            };

            return View(model);
        }

        [HttpPost]
        [Route("Cars/Approve/{carId}")]
        public async Task<IActionResult> ApproveCar(Guid carId)
        {
            try
            {
                Console.WriteLine($"ApproveCar: Approving car {carId}");
                
                var command = new AdminApproveCarCommand { CarId = carId, IsApproved = true, Status = "Approved" };
                var result = await _mediator.Send(command);
                
                Console.WriteLine($"ApproveCar: Result = {result}");
                
                if (result)
                {
                    TempData["Success"] = "Car approved successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to approve car.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error approving car: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = $"Error approving car: {ex.Message}";
            }

            return RedirectToAction("Cars");
        }

        [HttpPost]
        [Route("Cars/Reject/{carId}")]
        public async Task<IActionResult> RejectCar(Guid carId)
        {
            try
            {
                Console.WriteLine($"RejectCar: Rejecting car {carId}");
                
                var command = new AdminApproveCarCommand { CarId = carId, IsApproved = false, Status = "Rejected" };
                var result = await _mediator.Send(command);
                
                Console.WriteLine($"RejectCar: Result = {result}");
                
                if (result)
                {
                    TempData["Success"] = "Car rejected successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to reject car.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rejecting car: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = $"Error rejecting car: {ex.Message}";
            }

            return RedirectToAction("Cars");
        }

        [HttpGet]
        [Route("KYC")]
        public async Task<IActionResult> KYC()
        {
            var kycUploads = await _mediator.Send(new GetAllKYCUploadsQuery());
            return View(_mapper.Map<List<KYCUploadDto>>(kycUploads));
        }

        [HttpPost]
        [Route("KYC/Approve/{kycId}")]
        public async Task<IActionResult> ApproveKYC(Guid kycId)
        {
            try
            {
                // Log the approval attempt
                Console.WriteLine($"Attempting to approve KYC: {kycId}");
                
                var command = new AdminApproveOrRejectKYCCommand 
                { 
                    KYCId = kycId, 
                    IsApproved = true,
                    NewStatus = "Approved"
                };
                
                var result = await _mediator.Send(command);
                Console.WriteLine($"KYC approval result: {result}");

                if (result)
                {
                    TempData["Success"] = "KYC approved successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to approve KYC. It may already be processed.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error approving KYC: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = "Error approving KYC: " + ex.Message;
            }

            return RedirectToAction("KYC");
        }

        [HttpPost]
        [Route("KYC/Reject/{kycId}")]
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
        [Route("KYC/View/{kycId}")]
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
            return View(customers);
        }

        #region Enhanced Car Management

        [HttpGet]
        [Route("Cars/Details/{carId}")]
        public async Task<IActionResult> GetCarDetails(Guid carId)
        {
            try
            {
                var car = await _mediator.Send(new GetCarByIdQuery(carId));
                if (car == null)
                {
                    return NotFound();
                }

                var html = $@"
                    <div class='row'>
                        <div class='col-md-4'>
                            <div class='car-image-container'>
                                {(string.IsNullOrEmpty(car.ImagePath) ? 
                                    "<div class='car-placeholder-large'><i class='bi bi-car-front'></i></div>" : 
                                    $"<img src='{car.ImagePath}' alt='{car.Name}' class='img-fluid rounded' onerror='this.style.display=\"none\"; this.nextElementSibling.style.display=\"block\";'><div class='car-placeholder-large' style='display:none;'><i class='bi bi-car-front'></i></div>")}
                            </div>
                        </div>
                        <div class='col-md-8'>
                            <h4>{car.Name} {car.Model}</h4>
                            <p class='text-muted'>{car.Year} • {car.Transmission} • {car.FuelType}</p>
                            <p><strong>Description:</strong> {car.Description}</p>
                            <p><strong>Features:</strong> {car.Features}</p>
                            <p><strong>Rate per Day:</strong> ${car.RatePerDay}</p>
                            <p><strong>Available From:</strong> {car.AvailableFrom:MMM dd, yyyy}</p>
                            <p><strong>Available To:</strong> {car.AvailableTo:MMM dd, yyyy}</p>
                            <hr>
                            <h6>Owner Information</h6>
                            <p><strong>Name:</strong> {car.OwnerName}</p>
                            <p><strong>Email:</strong> {car.OwnerEmail}</p>
                        </div>
                    </div>";

                return Json(new { html });
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error loading car details" });
            }
        }

        #endregion

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

        #region Admin Car Management (Add/Edit/Delete)

        [HttpGet]
        [Route("Cars/Add")]
        public async Task<IActionResult> AddCar()
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery());
            var model = new AdminAddCarViewModel
            {
                AvailableOwners = customers.Select(c => new OwnerOption
                {
                    Id = c.Id,
                    Name = c.FullName,
                    Email = c.Email
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Route("Cars/Add")]
        public async Task<IActionResult> AddCar([FromForm] AdminAddCarViewModel model)
        {
            // Debug: Log raw form data first
            Console.WriteLine("Raw form data:");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"{key}: {Request.Form[key]}");
            }
            
            // Try manual model binding if automatic binding fails
            if (string.IsNullOrEmpty(model.Name) && Request.Form.ContainsKey("Name"))
            {
                Console.WriteLine("Manual model binding - form data detected but model is empty");
                model.Name = Request.Form["Name"].ToString();
                model.Model = Request.Form["Model"].ToString();
                if (int.TryParse(Request.Form["Year"], out int year))
                    model.Year = year;
                model.Transmission = Request.Form["Transmission"].ToString();
                model.FuelType = Request.Form["FuelType"].ToString();
                if (decimal.TryParse(Request.Form["RatePerDay"], out decimal rate))
                    model.RatePerDay = rate;
                model.Description = Request.Form["Description"].ToString();
                model.Features = Request.Form["Features"].ToString();
                if (Guid.TryParse(Request.Form["OwnerId"], out Guid ownerId))
                    model.OwnerId = ownerId;
                if (DateTime.TryParse(Request.Form["AvailableFrom"], out DateTime availableFrom))
                    model.AvailableFrom = availableFrom;
                if (DateTime.TryParse(Request.Form["AvailableTo"], out DateTime availableTo))
                    model.AvailableTo = availableTo;
                
                // Handle image file from form
                if (Request.Form.Files.Count > 0)
                {
                    model.ImageFile = Request.Form.Files[0];
                    Console.WriteLine($"Manual binding - Image file: {model.ImageFile?.FileName}, Size: {model.ImageFile?.Length}");
                }
                
                Console.WriteLine("Manual binding completed");
                
                // Clear ModelState and re-validate after manual binding
                ModelState.Clear();
                TryValidateModel(model);
            }
            
            // Debug: Log received model values
            Console.WriteLine($"AddCar POST - Model values:");
            Console.WriteLine($"Name: '{model.Name}'");
            Console.WriteLine($"Model: '{model.Model}'");
            Console.WriteLine($"Year: {model.Year}");
            Console.WriteLine($"Transmission: '{model.Transmission}'");
            Console.WriteLine($"FuelType: '{model.FuelType}'");
            Console.WriteLine($"RatePerDay: {model.RatePerDay}");
            Console.WriteLine($"Description: '{model.Description}'");
            Console.WriteLine($"Features: '{model.Features}'");
            Console.WriteLine($"OwnerId: {model.OwnerId}");
            Console.WriteLine($"AvailableFrom: {model.AvailableFrom}");
            Console.WriteLine($"AvailableTo: {model.AvailableTo}");
            Console.WriteLine($"ImageFile: {(model.ImageFile != null ? $"{model.ImageFile.FileName} ({model.ImageFile.Length} bytes)" : "null")}");
            
            // Debug: Log ModelState errors
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                
                // Debug: Log ModelState values
                Console.WriteLine("ModelState values:");
                foreach (var key in ModelState.Keys)
                {
                    var value = ModelState[key];
                    Console.WriteLine($"Key: {key}, Value: {value.AttemptedValue}, IsValid: {value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid}");
                }
            }
            
            if (!ModelState.IsValid)
            {
                var customers = await _mediator.Send(new GetAllCustomersQuery());
                model.AvailableOwners = customers.Select(c => new OwnerOption
                {
                    Id = c.Id,
                    Name = c.FullName,
                    Email = c.Email
                }).ToList();
                return View(model);
            }

            try
            {
                // Handle image upload
                string? imagePath = null;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Create unique filename
                    var fileName = $"{Guid.NewGuid()}_{model.ImageFile.FileName}";
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cars");
                    
                    // Ensure directory exists
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }
                    
                    var filePath = Path.Combine(uploadsPath, fileName);
                    
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }
                    
                    imagePath = fileName; // Store just the filename, not the full path
                    Console.WriteLine($"Image saved: {fileName} to {filePath}");
                }

                var command = new RegisterCarCommand
                {
                    Name = model.Name,
                    Model = model.Model,
                    Year = model.Year,
                    Transmission = model.Transmission,
                    FuelType = model.FuelType,
                    RatePerDay = model.RatePerDay,
                    Description = model.Description,
                    Features = model.Features,
                    AvailableFrom = model.AvailableFrom,
                    AvailableTo = model.AvailableTo,
                    OwnerId = model.OwnerId,
                    ImagePath = imagePath
                };

                var carId = await _mediator.Send(command);
                TempData["Success"] = "Car added successfully!";
                return RedirectToAction("Cars");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error adding car: " + ex.Message;
                var customers = await _mediator.Send(new GetAllCustomersQuery());
                model.AvailableOwners = customers.Select(c => new OwnerOption
                {
                    Id = c.Id,
                    Name = c.FullName,
                    Email = c.Email
                }).ToList();
                return View(model);
            }
        }

        [HttpGet]
        [Route("Cars/Edit/{carId}")]
        public async Task<IActionResult> EditCar(Guid carId)
        {
            try
            {
                // Log the car ID for debugging
                Console.WriteLine($"Attempting to edit car with ID: {carId}");
                
                var car = await _mediator.Send(new GetCarByIdQuery(carId));
                
                if (car == null)
                {
                    TempData["Error"] = $"Car with ID {carId} not found.";
                    return RedirectToAction("Cars");
                }
                
                var customers = await _mediator.Send(new GetAllCustomersQuery());

                var model = new AdminEditCarViewModel
                {
                    CarId = car.CarId,
                    Name = car.Name,
                    Model = car.Model,
                    Year = car.Year,
                    Transmission = car.Transmission,
                    FuelType = car.FuelType,
                    RatePerDay = car.RatePerDay,
                    Description = car.Description,
                    Features = car.Features,
                    AvailableFrom = car.AvailableFrom,
                    AvailableTo = car.AvailableTo,
                    OwnerId = car.OwnerId,
                    OwnerName = car.OwnerName,
                    CurrentImagePath = car.ImagePath,
                    ApprovalStatus = car.ApprovalStatus,
                    AvailableOwners = customers.Select(c => new OwnerOption
                    {
                        Id = c.Id,
                        Name = c.FullName,
                        Email = c.Email
                    }).ToList(),
                    AvailableStatuses = new List<StatusOption>
                    {
                        new StatusOption { Value = "Pending", Text = "Pending" },
                        new StatusOption { Value = "Approved", Text = "Approved" },
                        new StatusOption { Value = "Rejected", Text = "Rejected" }
                    }
                };

                return View(model);
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = $"Car not found: {ex.Message}";
                return RedirectToAction("Cars");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading car: {ex.Message}";
                return RedirectToAction("Cars");
            }
        }

        [HttpPost]
        [Route("Cars/Edit/{carId}")]
        public async Task<IActionResult> EditCar(Guid carId, AdminEditCarViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var customers = await _mediator.Send(new GetAllCustomersQuery());
                model.AvailableOwners = customers.Select(c => new OwnerOption
                {
                    Id = c.Id,
                    Name = c.FullName,
                    Email = c.Email
                }).ToList();
                model.AvailableStatuses = new List<StatusOption>
                {
                    new StatusOption { Value = "Pending", Text = "Pending" },
                    new StatusOption { Value = "Approved", Text = "Approved" },
                    new StatusOption { Value = "Rejected", Text = "Rejected" }
                };
                return View(model);
            }

            try
            {
                // Get approval status ID based on the status name
                var approvalStatusId = await GetApprovalStatusId(model.ApprovalStatus);

                var command = new UpdateCarCommand
                {
                    CarId = carId,
                    Name = model.Name,
                    Model = model.Model,
                    Year = model.Year,
                    Transmission = model.Transmission,
                    FuelType = model.FuelType,
                    RatePerDay = model.RatePerDay,
                    Description = model.Description,
                    Features = model.Features,
                    AvailableFrom = model.AvailableFrom,
                    AvailableTo = model.AvailableTo,
                    CarApprovalStatusId = approvalStatusId
                };

                var success = await _mediator.Send(command);
                if (success)
                {
                    TempData["Success"] = "Car updated successfully!";
                    return RedirectToAction("Cars");
                }
                else
                {
                    TempData["Error"] = "Failed to update car.";
                    return RedirectToAction("EditCar", new { carId });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating car: " + ex.Message;
                return RedirectToAction("EditCar", new { carId });
            }
        }

        [HttpPost]
        [Route("Cars/Delete/{carId}")]
        public async Task<IActionResult> DeleteCar(Guid carId)
        {
            try
            {
                var command = new DeleteCarCommand { CarId = carId };
                var success = await _mediator.Send(command);

                if (success)
                {
                    TempData["Success"] = "Car deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete car.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting car: " + ex.Message;
            }

            return RedirectToAction("Cars");
        }

        private async Task<Guid> GetApprovalStatusId(string statusName)
        {
            // This would typically come from your database
            // For now, returning a placeholder GUID
            return Guid.NewGuid();
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
    }
}
