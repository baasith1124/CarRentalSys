using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Payments.Commands.CreatePayment
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Guid>
    {
        private readonly IPaymentRepository _paymentRepository;

        public CreatePaymentCommandHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Guid> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = new Payment
            {
                BookingId = request.BookingId,
                Amount = request.Amount,
                Method = Enum.TryParse<PaymentMethod>(request.Method, out var method) ? method : PaymentMethod.Stripe,
                StripeTxnId = request.StripeTxnId,
                PaidAt = DateTime.UtcNow
            };

            return await _paymentRepository.CreatePaymentAsync(payment, cancellationToken);
        }
    }
}
