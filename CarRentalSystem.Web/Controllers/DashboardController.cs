using AutoMapper;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingsByCustomer;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingById;
using CarRentalSystem.Application.Features.Customers.Queries.GetCustomerById;
using CarRentalSystem.Web.ViewModels.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalSystem.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public DashboardController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var customer = await _mediator.Send(new GetCustomerByIdQuery(Guid.Parse(userId)));
            var bookings = await _mediator.Send(new GetBookingsByCustomerQuery(Guid.Parse(userId)));

            var dashboardData = new UserDashboardViewModel
            {
                Customer = customer,
                RecentBookings = bookings.Take(5).ToList(),
                TotalBookings = bookings.Count,
                ActiveRentals = bookings.Count(b => b.BookingStatus == "Confirmed" || b.BookingStatus == "In Progress"),
                TotalSpent = bookings.Where(b => b.PaymentStatus == "Paid").Sum(b => b.TotalCost),
                Savings = bookings.Where(b => b.PaymentStatus == "Paid").Sum(b => b.TotalCost) * 0.15m // 15% savings vs traditional rental
            };

            return View(dashboardData);
        }

        [HttpGet]
        [Route("Dashboard/Profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var customer = await _mediator.Send(new GetCustomerByIdQuery(Guid.Parse(userId)));
            return View(customer);
        }

        [HttpGet]
        [Route("Dashboard/Bookings")]
        public async Task<IActionResult> Bookings(bool? paymentSuccess = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

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

            return View(booking);
        }
    }
}
