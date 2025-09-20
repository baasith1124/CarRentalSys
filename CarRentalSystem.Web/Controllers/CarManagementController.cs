using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Features.Cars.Commands.AdminApproveCar;
using CarRentalSystem.Application.Features.Cars.Commands.RegisterCar;
using CarRentalSystem.Application.Features.Cars.Commands.UpdateCar;
using CarRentalSystem.Application.Features.Cars.Commands.DeleteCar;
using CarRentalSystem.Application.Features.Cars.Queries.GetCarsByApprovalStatus;
using CarRentalSystem.Application.Features.Cars.Queries.GetAllCars;
using CarRentalSystem.Application.Features.Cars.Queries.GetCarById;
using CarRentalSystem.Application.Features.CarApproval.Queries.GetAllCarApprovalStatuses;
using CarRentalSystem.Application.Features.Customers.Queries.GetAllCustomers;
using CarRentalSystem.Web.ViewModels.Admin;
using CarRentalSystem.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Web.Controllers
{
    [Route("Admin/CarManagement")]
    [Authorize(Roles = "Admin")]
    public class CarManagementController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ICarRepository _carRepository;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarManagementController(
            IMediator mediator, 
            IMapper mapper, 
            ICarRepository carRepository, 
            IWebHostEnvironment env, 
            UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _mapper = mapper;
            _carRepository = carRepository;
            _env = env;
            _userManager = userManager;
        }

        #region Index - List All Cars

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var pendingCars = await _carRepository.GetCarsByApprovalStatusWithOwnerAsync("Pending", CancellationToken.None);
                var approvedCars = await _carRepository.GetCarsByApprovalStatusWithOwnerAsync("Approved", CancellationToken.None);
                var rejectedCars = await _carRepository.GetCarsByApprovalStatusWithOwnerAsync("Rejected", CancellationToken.None);

                var model = new CarManagementViewModel
                {
                    PendingCars = pendingCars,
                    ApprovedCars = approvedCars,
                    RejectedCars = rejectedCars
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading cars: {ex.Message}";
                return View(new CarManagementViewModel());
            }
        }

        #endregion

        #region Create Car


        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Debug: Log that POST method was called
                Console.WriteLine("=== CarManagement Create POST method called ===");
                
                // Read form data
                var name = Request.Form["Name"].FirstOrDefault();
                var model = Request.Form["Model"].FirstOrDefault();
                var yearStr = Request.Form["Year"].FirstOrDefault();
                var transmission = Request.Form["Transmission"].FirstOrDefault();
                var fuelType = Request.Form["FuelType"].FirstOrDefault();
                var ratePerDayStr = Request.Form["RatePerDay"].FirstOrDefault();
                var ownerIdStr = Request.Form["OwnerId"].FirstOrDefault();
                var availableFromStr = Request.Form["AvailableFrom"].FirstOrDefault();
                var availableToStr = Request.Form["AvailableTo"].FirstOrDefault();
                var description = Request.Form["Description"].FirstOrDefault();
                var features = Request.Form["Features"].FirstOrDefault();
                var imageFile = Request.Form.Files["ImageFile"];

                Console.WriteLine($"Form data - Name: {name}, Model: {model}, Year: {yearStr}");

                // Validate required fields
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(yearStr) ||
                    string.IsNullOrEmpty(transmission) || string.IsNullOrEmpty(fuelType) || string.IsNullOrEmpty(ratePerDayStr) ||
                    string.IsNullOrEmpty(ownerIdStr) || string.IsNullOrEmpty(availableFromStr) || string.IsNullOrEmpty(availableToStr))
                {
                    var errorMsg = "All required fields must be filled.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                // Parse values
                if (!int.TryParse(yearStr, out int year))
                {
                    var errorMsg = "Invalid year value.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                if (!decimal.TryParse(ratePerDayStr, out decimal ratePerDay))
                {
                    var errorMsg = "Invalid rate per day value.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                if (!Guid.TryParse(ownerIdStr, out Guid ownerId))
                {
                    var errorMsg = "Invalid owner ID.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                if (!DateTime.TryParse(availableFromStr, out DateTime availableFrom))
                {
                    var errorMsg = "Invalid available from date.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                if (!DateTime.TryParse(availableToStr, out DateTime availableTo))
                {
                    var errorMsg = "Invalid available to date.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                // Handle image upload
                string? imagePath = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var uploadsPath = Path.Combine(_env.WebRootPath, "images", "cars");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }
                    var filePath = Path.Combine(uploadsPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    imagePath = fileName;
                }

                var command = new RegisterCarCommand
                {
                    Name = name,
                    Model = model,
                    Year = year,
                    Transmission = transmission,
                    FuelType = fuelType,
                    RatePerDay = ratePerDay,
                    Description = description,
                    Features = features,
                    AvailableFrom = availableFrom,
                    AvailableTo = availableTo,
                    OwnerId = ownerId,
                    ImagePath = imagePath
                };

                Console.WriteLine("Sending RegisterCarCommand to mediator...");
                var carId = await _mediator.Send(command);
                Console.WriteLine($"Car created successfully with ID: {carId}");
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Car created successfully!" });
                }
                
                TempData["Success"] = "Car created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating car: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Error creating car: " + ex.Message });
                }
                
                TempData["Error"] = $"Error creating car: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Edit Car


        [HttpPost]
        [Route("Edit/{carId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid carId)
        {
            try
            {
                Console.WriteLine($"=== CarManagement Edit POST method called for carId: {carId} ===");
                
                // Read form data
                var name = Request.Form["Name"].FirstOrDefault();
                var model = Request.Form["Model"].FirstOrDefault();
                var yearStr = Request.Form["Year"].FirstOrDefault();
                var transmission = Request.Form["Transmission"].FirstOrDefault();
                var fuelType = Request.Form["FuelType"].FirstOrDefault();
                var ratePerDayStr = Request.Form["RatePerDay"].FirstOrDefault();
                var ownerIdStr = Request.Form["OwnerId"].FirstOrDefault();
                var availableFromStr = Request.Form["AvailableFrom"].FirstOrDefault();
                var availableToStr = Request.Form["AvailableTo"].FirstOrDefault();
                var description = Request.Form["Description"].FirstOrDefault();
                var features = Request.Form["Features"].FirstOrDefault();
                var approvalStatus = Request.Form["ApprovalStatus"].FirstOrDefault();
                var currentImagePath = Request.Form["CurrentImagePath"].FirstOrDefault();
                var imageFile = Request.Form.Files["ImageFile"];

                Console.WriteLine($"Edit form data - Name: {name}, Model: {model}, Year: {yearStr}");

                // Validate required fields
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(yearStr) ||
                    string.IsNullOrEmpty(transmission) || string.IsNullOrEmpty(fuelType) || string.IsNullOrEmpty(ratePerDayStr) ||
                    string.IsNullOrEmpty(ownerIdStr) || string.IsNullOrEmpty(availableFromStr) || string.IsNullOrEmpty(availableToStr))
                {
                    var errorMsg = "All required fields must be filled.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                // Parse values
                if (!int.TryParse(yearStr, out int year))
                {
                    var errorMsg = "Invalid year value.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                if (!decimal.TryParse(ratePerDayStr, out decimal ratePerDay))
                {
                    var errorMsg = "Invalid rate per day value.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                if (!Guid.TryParse(ownerIdStr, out Guid ownerId))
                {
                    var errorMsg = "Invalid owner ID.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                if (!DateTime.TryParse(availableFromStr, out DateTime availableFrom))
                {
                    var errorMsg = "Invalid available from date.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                if (!DateTime.TryParse(availableToStr, out DateTime availableTo))
                {
                    var errorMsg = "Invalid available to date.";
                    Console.WriteLine(errorMsg);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    TempData["Error"] = errorMsg;
                    return RedirectToAction("Index");
                }

                // Handle image upload
                string? imagePath = currentImagePath?.Replace("/images/cars/", "");
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var uploadsPath = Path.Combine(_env.WebRootPath, "images", "cars");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }
                    var filePath = Path.Combine(uploadsPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    imagePath = fileName;
                }

                // Get approval status ID (default to "Approved" if not specified)
                var approvalStatusId = await GetApprovalStatusId(approvalStatus ?? "Approved");

                var command = new UpdateCarCommand
                {
                    CarId = carId,
                    Name = name,
                    Model = model,
                    Year = year,
                    Transmission = transmission,
                    FuelType = fuelType,
                    RatePerDay = ratePerDay,
                    Description = description,
                    Features = features,
                    AvailableFrom = availableFrom,
                    AvailableTo = availableTo,
                    CarApprovalStatusId = approvalStatusId,
                    ImagePath = imagePath
                };

                Console.WriteLine("Sending UpdateCarCommand to mediator...");
                var success = await _mediator.Send(command);
                Console.WriteLine($"Car update result: {success}");
                
                if (success)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Car updated successfully!" });
                    }
                    
                    TempData["Success"] = "Car updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Failed to update car." });
                    }
                    
                    TempData["Error"] = "Failed to update car.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating car: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Error updating car: " + ex.Message });
                }
                
                TempData["Error"] = $"Error updating car: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Delete Car

        [HttpPost]
        [Route("Delete/{carId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid carId)
        {
            try
            {
                // Get car details to handle image deletion
                var car = await _mediator.Send(new GetCarByIdQuery(carId));
                if (car != null && !string.IsNullOrEmpty(car.ImagePath))
                {
                    try
                    {
                        var imagePath = Path.Combine(_env.WebRootPath, "images", "cars", car.ImagePath);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    catch (Exception imgEx)
                    {
                        // Continue with deletion even if image deletion fails
                        Console.WriteLine($"Error deleting car image: {imgEx.Message}");
                    }
                }

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
                TempData["Error"] = $"Error deleting car: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Approve/Reject Car

        [HttpPost]
        [Route("Approve/{carId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid carId)
        {
            try
            {
                var command = new AdminApproveCarCommand 
                { 
                    CarId = carId, 
                    IsApproved = true, 
                    Status = "Approved" 
                };
                var result = await _mediator.Send(command);
                
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
                TempData["Error"] = $"Error approving car: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Reject/{carId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid carId)
        {
            try
            {
                var command = new AdminApproveCarCommand 
                { 
                    CarId = carId, 
                    IsApproved = false, 
                    Status = "Rejected" 
                };
                var result = await _mediator.Send(command);
                
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
                TempData["Error"] = $"Error rejecting car: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Car Details

        [HttpGet]
        [Route("Details/{carId}")]
        public async Task<IActionResult> Details(Guid carId)
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
                                    $"<img src='/images/cars/{car.ImagePath}' alt='{car.Name}' class='img-fluid rounded' onerror='this.style.display=\"none\"; this.nextElementSibling.style.display=\"block\";'><div class='car-placeholder-large' style='display:none;'><i class='bi bi-car-front'></i></div>")}
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

        #region Modal Support Methods

        [HttpGet]
        [Route("GetOwners")]
        public async Task<IActionResult> GetOwners()
        {
            try
            {
                Console.WriteLine("GetOwners method called");
                
                // Check if user is authenticated
                if (!User.Identity.IsAuthenticated)
                {
                    Console.WriteLine("User not authenticated");
                    return Json(new { error = "User not authenticated" });
                }

                // Check if user is admin
                if (!User.IsInRole("Admin"))
                {
                    Console.WriteLine("User not admin");
                    return Json(new { error = "Access denied" });
                }

                var allUsers = _userManager.Users.ToList();
                var owners = allUsers.Select(u => new
                {
                    id = u.Id,
                    name = u.FullName ?? u.UserName ?? u.Email,
                    email = u.Email
                }).ToList();

                Console.WriteLine($"Returning {owners.Count} owners");
                return Json(owners);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading owners: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { error = "Error loading owners: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("GetCarData/{carId}")]
        public async Task<IActionResult> GetCarData(Guid carId)
        {
            try
            {
                var car = await _mediator.Send(new GetCarByIdQuery(carId));
                if (car == null)
                {
                    return Json(new { success = false, error = "Car not found" });
                }

                var carData = new
                {
                    carId = car.CarId,
                    name = car.Name,
                    model = car.Model,
                    year = car.Year,
                    transmission = car.Transmission,
                    fuelType = car.FuelType,
                    ratePerDay = car.RatePerDay,
                    ownerId = car.OwnerId,
                    availableFrom = car.AvailableFrom.ToString("yyyy-MM-dd"),
                    availableTo = car.AvailableTo.ToString("yyyy-MM-dd"),
                    description = car.Description,
                    features = car.Features,
                    imagePath = car.ImagePath
                };

                return Json(new { success = true, car = carData });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading car data: {ex.Message}");
                return Json(new { success = false, error = "Error loading car data" });
            }
        }

        #endregion

        #region Helper Methods

        private async Task<Guid> GetApprovalStatusId(string statusName)
        {
            try
            {
                var statusId = await _carRepository.GetStatusIdByNameAsync(statusName, CancellationToken.None);
                return statusId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Car approval status '{statusName}' not found in database.");
            }
        }

        #endregion
    }
}
