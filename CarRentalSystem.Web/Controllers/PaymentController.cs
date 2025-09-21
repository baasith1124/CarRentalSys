using AutoMapper;
using CarRentalSystem.Application.Features.Payments.Commands.CreatePayment;
using CarRentalSystem.Application.Features.Invoices.Commands.CreateInvoice;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingById;
using CarRentalSystem.Application.Features.Payments.Queries.GetPaymentById;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Web.ViewModels.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace CarRentalSystem.Web.Controllers
{
    [Authorize]
    [Route("Payment")]
    public class PaymentController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IBookingRepository _bookingRepository;
        private readonly IPdfService _pdfService;

        public PaymentController(IMediator mediator, IMapper mapper, IConfiguration configuration, IBookingRepository bookingRepository, IPdfService pdfService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _configuration = configuration;
            _bookingRepository = bookingRepository;
            _pdfService = pdfService;
            
            // Initialize Stripe
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        [HttpGet]
        [Route("Checkout/{bookingId}")]
        public async Task<IActionResult> Checkout(Guid bookingId)
        {
            try
            {
                // Get booking details
                var booking = await _mediator.Send(new GetBookingByIdQuery { BookingId = bookingId });
                
                if (booking == null)
                {
                    TempData["Error"] = "Booking not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Verify that the booking belongs to the current user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null || booking.CustomerId != Guid.Parse(userId))
                {
                    TempData["Error"] = "You can only pay for your own bookings.";
                    return RedirectToAction("Index", "Home");
                }

                var model = new PaymentCheckoutViewModel
                {
                    BookingId = bookingId,
                    Amount = booking.TotalCost,
                    CarName = booking.CarName,
                    PickupDate = booking.PickupDate,
                    ReturnDate = booking.ReturnDate,
                    CustomerName = booking.CustomerName
                };

                ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading booking details.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [Route("CreatePaymentIntent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
        {
            try
            {
                var service = new PaymentIntentService();
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(request.Amount * 100), // Convert to cents
                    Currency = "usd",
                    Metadata = new Dictionary<string, string>
                    {
                        { "bookingId", request.BookingId.ToString() },
                        { "userId", User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "" }
                    }
                };

                var paymentIntent = await service.CreateAsync(options);

                return Json(new { clientSecret = paymentIntent.ClientSecret });
            }
            catch (StripeException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost]
        [Route("ConfirmPayment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(request.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    // Create payment record
                    var createPaymentCommand = new CreatePaymentCommand
                    {
                        BookingId = request.BookingId,
                        Amount = request.Amount,
                        Method = "Stripe",
                        StripeTxnId = paymentIntent.Id
                    };

                    var paymentId = await _mediator.Send(createPaymentCommand);

                    // Update booking status to "Confirmed" and payment status to "Paid"
                    var confirmedStatusId = await _bookingRepository.GetBookingStatusIdByNameAsync("Confirmed", CancellationToken.None);
                    var paidStatusId = await _bookingRepository.GetPaymentStatusIdByNameAsync("Paid", CancellationToken.None);
                    
                    var updateBookingStatusCommand = new CarRentalSystem.Application.Features.Bookings.Commands.UpdateBookingStatus.UpdateBookingStatusCommand
                    {
                        BookingId = request.BookingId,
                        BookingStatusId = confirmedStatusId,
                        PaymentStatusId = paidStatusId
                    };

                    await _mediator.Send(updateBookingStatusCommand);

                    // Check if booking has coordinates, if not, try to geocode the locations
                    var booking = await _mediator.Send(new GetBookingByIdQuery { BookingId = request.BookingId });
                    if (booking != null && (!booking.PickupLatitude.HasValue || !booking.DropLatitude.HasValue))
                    {
                        // Try to geocode the locations if coordinates are missing
                        await TryGeocodeBookingLocations(request.BookingId, booking.PickupLocation, booking.DropLocation);
                    }

                    // Create invoice
                    var createInvoiceCommand = new CreateInvoiceCommand
                    {
                        PaymentId = paymentId,
                        FilePath = $"/invoices/invoice_{paymentId}.pdf" // You'll need to generate this
                    };

                    await _mediator.Send(createInvoiceCommand);

                    return Json(new { success = true, paymentId = paymentId.ToString(), bookingId = request.BookingId.ToString() });
                }
                else
                {
                    return BadRequest(new { error = "Payment was not successful" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet]
        [Route("Success/{paymentId}")]
        public async Task<IActionResult> PaymentSuccess(Guid paymentId)
        {
            try
            {
                // Get payment details
                var payment = await _mediator.Send(new GetPaymentByIdQuery(paymentId));
                
                if (payment == null)
                {
                    TempData["Error"] = "Payment not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Get booking details
                var booking = await _mediator.Send(new GetBookingByIdQuery { BookingId = payment.BookingId });
                
                if (booking == null)
                {
                    TempData["Error"] = "Booking not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Verify that the booking belongs to the current user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null || booking.CustomerId != Guid.Parse(userId))
                {
                    TempData["Error"] = "You can only view your own bookings.";
                    return RedirectToAction("Index", "Home");
                }

                // Pass data to view
                ViewBag.PaymentId = paymentId;
                ViewBag.BookingId = booking.BookingId;
                ViewBag.BookingStatus = booking.BookingStatus;
                ViewBag.PaymentStatus = booking.PaymentStatus;
                ViewBag.CarName = booking.CarName;
                ViewBag.CustomerName = booking.CustomerName;
                ViewBag.PickupDate = booking.PickupDate;
                ViewBag.ReturnDate = booking.ReturnDate;
                ViewBag.TotalCost = booking.TotalCost;
                ViewBag.ShowBookingLink = true;
                
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading payment details.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Route("Cancel")]
        public IActionResult PaymentCancel()
        {
            return View();
        }

        [HttpGet]
        [Route("DownloadReceipt/{bookingId}")]
        public async Task<IActionResult> DownloadReceipt(Guid bookingId)
        {
            try
            {
                // Get booking details
                var booking = await _mediator.Send(new GetBookingByIdQuery { BookingId = bookingId });
                
                if (booking == null)
                {
                    TempData["Error"] = "Booking not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Verify that the booking belongs to the current user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null || booking.CustomerId != Guid.Parse(userId))
                {
                    TempData["Error"] = "You can only download receipts for your own bookings.";
                    return RedirectToAction("Index", "Home");
                }

                // Generate receipt PDF
                var pdfBytes = await _pdfService.GenerateReceiptPdfAsync(booking, $"RCP-{bookingId.ToString().Substring(0, 8)}", booking.TotalCost);
                
                var fileName = $"Receipt_{bookingId.ToString().Substring(0, 8)}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating receipt.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Helper method to geocode booking locations
        private async Task TryGeocodeBookingLocations(Guid bookingId, string pickupLocation, string dropLocation)
        {
            try
            {
                // This is a simple implementation - in production, you'd want to use a proper geocoding service
                // For now, we'll use a basic approach with sample coordinates
                
                // You could integrate with Google Geocoding API here
                // For demonstration, we'll use some sample coordinates
                var sampleCoordinates = new Dictionary<string, (double lat, double lng)>
                {
                    { "New York", (40.7128, -74.0060) },
                    { "Los Angeles", (34.0522, -118.2437) },
                    { "Chicago", (41.8781, -87.6298) },
                    { "Houston", (29.7604, -95.3698) },
                    { "Phoenix", (33.4484, -112.0740) },
                    { "Philadelphia", (39.9526, -75.1652) },
                    { "San Antonio", (29.4241, -98.4936) },
                    { "San Diego", (32.7157, -117.1611) },
                    { "Dallas", (32.7767, -96.7970) },
                    { "San Jose", (37.3382, -121.8863) }
                };

                // Try to find coordinates for pickup location
                var pickupCoords = sampleCoordinates.FirstOrDefault(x => 
                    pickupLocation.Contains(x.Key, StringComparison.OrdinalIgnoreCase));
                
                // Try to find coordinates for drop location
                var dropCoords = sampleCoordinates.FirstOrDefault(x => 
                    dropLocation.Contains(x.Key, StringComparison.OrdinalIgnoreCase));

                // Update booking with coordinates if found
                if (pickupCoords.Key != null || dropCoords.Key != null)
                {
                    // You would need to add an UpdateBookingCoordinates command here
                    // For now, we'll just log that we would update the coordinates
                    Console.WriteLine($"Would update booking {bookingId} with coordinates: Pickup: {pickupCoords.Value}, Drop: {dropCoords.Value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error geocoding booking locations: {ex.Message}");
            }
        }
    }

    public class CreatePaymentIntentRequest
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
    }

    public class ConfirmPaymentRequest
    {
        public string PaymentIntentId { get; set; } = null!;
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
    }
}

