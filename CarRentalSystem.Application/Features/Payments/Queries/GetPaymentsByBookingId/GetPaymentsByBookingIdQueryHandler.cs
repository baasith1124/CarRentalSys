using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Payments.Queries.GetPaymentsByBookingId
{
    public class GetPaymentsByBookingIdQueryHandler : IRequestHandler<GetPaymentsByBookingIdQuery, List<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentsByBookingIdQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<List<PaymentDto>> Handle(GetPaymentsByBookingIdQuery request, CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetPaymentsByBookingIdAsync(request.BookingId, cancellationToken);

            return payments.Select(p => new PaymentDto
            {
                PaymentId = p.PaymentId,
                BookingId = p.BookingId,
                Amount = p.Amount,
                Method = p.Method.ToString(),
                StripeTxnId = p.StripeTxnId,
                PaidAt = p.PaidAt
            }).ToList();
        }
    }
}
