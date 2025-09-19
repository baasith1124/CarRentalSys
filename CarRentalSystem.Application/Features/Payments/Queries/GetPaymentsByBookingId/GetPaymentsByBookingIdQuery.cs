using CarRentalSystem.Application.Contracts.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Payments.Queries.GetPaymentsByBookingId
{
    public class GetPaymentsByBookingIdQuery : IRequest<List<PaymentDto>>
    {
        public Guid BookingId { get; set; }

        public GetPaymentsByBookingIdQuery(Guid bookingId)
        {
            BookingId = bookingId;
        }
    }
}
