using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Payments.Queries.GetPaymentById
{
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(request.PaymentId, cancellationToken);

            if (payment == null) return null!;

            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                Method = payment.Method.ToString(),
                StripeTxnId = payment.StripeTxnId,
                PaidAt = payment.PaidAt
            };
        }
    }
}
