using AutoMapper;
using CarRentalSystem.Application.Contracts.Booking;
using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Application.Features.Cars.Commands.RegisterCar;
using CarRentalSystem.Application.Features.Cars.Queries.GetMyCars;
using CarRentalSystem.Application.Features.Cars.Queries.GetCarById;
using CarRentalSystem.Application.Features.Cars.Commands.UpdateCar;
using CarRentalSystem.Application.Features.Cars.Commands.DeleteCar;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingsByCarOwner;
using CarRentalSystem.Web.ViewModels.CarOwner;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalSystem.Web.Controllers
{
    [Authorize(Roles = "CarOwner")]
    [Route("CarOwner")]
    public class CarOwnerController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public CarOwnerController(IMediator mediator, IMapper mapper, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var myCars = await _mediator.Send(new GetMyCarsQuery(Guid.Parse(userId)));
            var bookings = await _mediator.Send(new GetBookingsByCarOwnerDtoQuery(Guid.Parse(userId)));

            var dashboardData = new CarOwnerDashboardViewModel
            {
                MyCars = _mapper.Map<List<CarDto>>(myCars),
                RecentBookings = bookings.Take(5).ToList(),
                TotalEarnings = bookings.Where(b => b.PaymentStatus == "Paid").Sum(b => b.TotalCost),
                TotalBookings = bookings.Count,
                ActiveCars = myCars.Count(c => c.CarApprovalStatus == "Approved")
            };

            return View(dashboardData);
        }

        [HttpGet]
        [Route("Cars")]
        public async Task<IActionResult> Cars()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var myCars = await _mediator.Send(new GetMyCarsQuery(Guid.Parse(userId)));
            return View(myCars);
        }

        [HttpGet]
        [Route("Cars/Register")]
        public IActionResult RegisterCar()
        {
            return View(new RegisterCarViewModel());
        }

        [HttpPost]
        [Route("Cars/Register")]
        public async Task<IActionResult> RegisterCar(RegisterCarViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Handle file uploads
            var imagePath = await SaveFile(model.CarImage, "cars");
            var documentPaths = new List<string>();

            if (model.Documents?.Any() == true)
            {
                foreach (var doc in model.Documents)
                {
                    var docPath = await SaveFile(doc, "documents");
                    documentPaths.Add(docPath);
                }
            }

            var command = new RegisterCarCommand
            {
                Name = model.Name,
                Model = model.Model,
                ImagePath = imagePath,
                AvailableFrom = model.AvailableFrom,
                AvailableTo = model.AvailableTo,
                OwnerId = Guid.Parse(userId),
                Description = model.Description,
                Features = model.Features,
                Year = model.Year ?? 0,
                Transmission = model.Transmission,
                FuelType = model.FuelType,
                RatePerDay = model.RatePerDay,
                Documents = model.Documents?.ToList() ?? new List<IFormFile>()
            };

            var carId = await _mediator.Send(command);

            TempData["Success"] = "Car registered successfully! It will be reviewed by our admin team.";
            return RedirectToAction("Cars");
        }

        [HttpGet]
        [Route("Cars/Edit/{carId}")]
        public async Task<IActionResult> EditCar(Guid carId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var car = await _mediator.Send(new GetCarByIdQuery(carId));
            if (car == null || car.OwnerId.ToString() != userId)
                return NotFound();

            var model = _mapper.Map<EditCarViewModel>(car);
            return View(model);
        }

        [HttpPost]
        [Route("Cars/Edit/{carId}")]
        public async Task<IActionResult> EditCar(Guid carId, EditCarViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var command = new UpdateCarCommand
            {
                CarId = carId,
                Name = model.Name,
                Model = model.Model,
                AvailableFrom = model.AvailableFrom,
                AvailableTo = model.AvailableTo,
                Description = model.Description,
                Features = model.Features,
                Year = model.Year ?? 0,
                Transmission = model.Transmission,
                FuelType = model.FuelType,
                RatePerDay = model.RatePerDay
            };

            await _mediator.Send(command);

            TempData["Success"] = "Car updated successfully!";
            return RedirectToAction("Cars");
        }

        [HttpPost]
        [Route("Cars/Delete/{carId}")]
        public async Task<IActionResult> DeleteCar(Guid carId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var car = await _mediator.Send(new GetCarByIdQuery(carId));
            if (car == null || car.OwnerId.ToString() != userId)
                return NotFound();

            var command = new DeleteCarCommand { CarId = carId };
            await _mediator.Send(command);

            TempData["Success"] = "Car deleted successfully!";
            return RedirectToAction("Cars");
        }

        [HttpGet]
        [Route("Bookings")]
        public async Task<IActionResult> Bookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var bookings = await _mediator.Send(new GetBookingsByCarOwnerDtoQuery(Guid.Parse(userId)));
            return View(bookings);
        }

        [HttpGet]
        [Route("Earnings")]
        public async Task<IActionResult> Earnings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var bookings = await _mediator.Send(new GetBookingsByCarOwnerDtoQuery(Guid.Parse(userId)));
            var earningsData = new EarningsViewModel
            {
                TotalEarnings = bookings.Where(b => b.PaymentStatus == "Paid").Sum(b => b.TotalCost),
                MonthlyEarnings = bookings.Where(b => b.PaymentStatus == "Paid" && b.PickupDate.Month == DateTime.Now.Month).Sum(b => b.TotalCost),
                PendingPayments = bookings.Where(b => b.PaymentStatus == "Pending").Sum(b => b.TotalCost),
                Bookings = bookings
            };

            return View(earningsData);
        }

        private async Task<string> SaveFile(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{fileName}";
        }
    }
}
