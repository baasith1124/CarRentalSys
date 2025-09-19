using AutoMapper;
using CarRentalSystem.Application.Features.Payments.Commands.CreatePayment;
using CarRentalSystem.Application.Features.Invoices.Commands.CreateInvoice;
using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingById;
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

        public PaymentController(IMediator mediator, IMapper mapper, IConfiguration configuration)
        {
            _mediator = mediator;
            _mapper = mapper;
            _configuration = configuration;
            
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
            ViewBag.PaymentId = paymentId;
            
            // Try to get the booking ID for this payment
            try
            {
                // You might need to implement a query to get booking by payment ID
                // For now, we'll use ViewBag to pass the payment ID
                ViewBag.ShowBookingLink = true;
            }
            catch
            {
                ViewBag.ShowBookingLink = false;
            }
            
            return View();
        }

        [HttpGet]
        [Route("Cancel")]
        public IActionResult PaymentCancel()
        {
            return View();
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
