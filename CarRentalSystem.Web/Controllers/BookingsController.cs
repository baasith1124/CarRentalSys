﻿﻿﻿﻿﻿﻿﻿﻿using CarRentalSystem.Application.Features.Bookings.Commands.CreateBooking;
using CarRentalSystem.Application.Features.Cars.Queries.GetCarById;
using CarRentalSystem.Application.Features.KYC.Queries.HasUploadedKYC;
using CarRentalSystem.Web.Extensions;
using CarRentalSystem.Web.ViewModels.Bookings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Web.Controllers
{
    [Authorize]
    [Route("Bookings")]
    public class BookingsController : Controller
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Index action for /Bookings - redirect to search
        [HttpGet("")]
        public IActionResult Index()
        {
            TempData["Info"] = "Please search for cars first to start the booking process.";
            return RedirectToAction("Index", "Home");
        }

        //  View Booking Summary
        [HttpGet("Book")]
        public async Task<IActionResult> Book(Guid carId, DateTime? pickupDate = null, DateTime? returnDate = null, 
                                            string? pickupLocation = null, string? dropLocation = null)
        {
            DateTime finalPickupDate, finalDropDate;
            string finalPickupLocation, finalDropLocation;
            
            // Check if custom parameters are provided, otherwise use session data
            if (pickupDate.HasValue && returnDate.HasValue && !string.IsNullOrEmpty(pickupLocation) && !string.IsNullOrEmpty(dropLocation))
            {
                // Use custom parameters and update session
                finalPickupDate = pickupDate.Value;
                finalDropDate = returnDate.Value;
                finalPickupLocation = pickupLocation;
                finalDropLocation = dropLocation;
                
                HttpContext.Session.SetString("PickupDate", finalPickupDate.ToString("o"));
                HttpContext.Session.SetString("DropDate", finalDropDate.ToString("o"));
                HttpContext.Session.SetString("PickupLocation", finalPickupLocation);
                HttpContext.Session.SetString("DropLocation", finalDropLocation);
            }
            else
            {
                // Use session data
                var pickupDateStr = HttpContext.Session.GetString("PickupDate");
                var dropDateStr = HttpContext.Session.GetString("DropDate");
                finalPickupLocation = HttpContext.Session.GetString("PickupLocation") ?? "";
                finalDropLocation = HttpContext.Session.GetString("DropLocation") ?? "";
                
                // Debug: Check if session data exists
                if (string.IsNullOrWhiteSpace(pickupDateStr) || string.IsNullOrWhiteSpace(dropDateStr))
                {
                    TempData["Error"] = "Please search for cars first to set pickup and drop dates, or use the custom booking form on the car details page.";
                    return RedirectToAction("Details", "Cars", new { id = carId });
                }
                
                finalPickupDate = DateTime.Parse(pickupDateStr);
                finalDropDate = DateTime.Parse(dropDateStr);
            }

            var car = await _mediator.Send(new GetCarByIdQuery(carId));

            if (car == null)
                return NotFound();

            // Get current user info and KYC status
            var userId = User.GetUserId();
            var hasKyc = await _mediator.Send(new HasUploadedKYCQuery(userId));
            
            // Get user information (you might need to add a query for this)
            var userName = User.Identity?.Name ?? "User";
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";

            var viewModel = new BookingSummaryViewModel
            {
                CarId = carId,
                Car = car,
                PickupDate = finalPickupDate,
                DropDate = finalDropDate,
                PickupLocation = finalPickupLocation,
                DropLocation = finalDropLocation,
                EstimatedCost = (decimal)(finalDropDate - finalPickupDate).TotalDays * car.RatePerDay,
                HasKYC = hasKyc,
                CustomerName = userName,
                CustomerEmail = userEmail
            };

            return View("BookingSummary", viewModel);
        }

        // Confirm Booking
        [HttpPost("ConfirmBooking")]
        public async Task<IActionResult> ConfirmBooking(ConfirmBookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Get the car info again
                var car = await _mediator.Send(new GetCarByIdQuery(model.CarId));

                if (car == null)
                    return NotFound();

                var summaryModel = new BookingSummaryViewModel
                {
                    CarId = model.CarId,
                    Car = car,
                    PickupDate = model.PickupDate,
                    DropDate = model.DropDate,
                    PickupLocation = model.PickupLocation,
                    DropLocation = model.DropLocation,
                    EstimatedCost = model.EstimatedCost
                };

                return View("BookingSummary", summaryModel); //  View expects this model
            }

            var userId = User.GetUserId(); // extension method you already have
            var hasKyc = await _mediator.Send(new HasUploadedKYCQuery(userId));

            if (!hasKyc)
            {
                TempData["Error"] = "KYC not found. Please upload your KYC documents to proceed.";
                return RedirectToAction("Upload", "KYC");
            }

            var command = new CreateBookingCommand
            {
                CarId = model.CarId,
                CustomerId = User.GetUserId(), // CustomerId should match UserId since they're the same in the database
                PickupDate = model.PickupDate,
                ReturnDate = model.DropDate,
                PickupLocation = model.PickupLocation,
                DropLocation = model.DropLocation,
                TotalCost = model.EstimatedCost,
                UserId = User.GetUserId() // custom extension method to get logged-in user Guid
            };

            var bookingId = await _mediator.Send(command);

            // Redirect to payment instead of confirmation
            return RedirectToAction("Checkout", "Payment", new { bookingId = bookingId });
        }

        // Confirmation Page
        [HttpGet("Confirmation")]
        public IActionResult Confirmation(Guid id)
        {
            ViewBag.BookingId = id;
            return View(); // Show simple confirmation with ID
        }
    }
}
