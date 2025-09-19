using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Payments.Commands.CreatePayment
{
    public class CreatePaymentCommand : IRequest<Guid>  // returns PaymentId
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Stripe";
        public string? StripeTxnId { get; set; }
    }
}
