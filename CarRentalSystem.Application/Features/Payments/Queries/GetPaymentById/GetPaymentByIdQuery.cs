using CarRentalSystem.Application.Contracts.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Payments.Queries.GetPaymentById
{
    public class GetPaymentByIdQuery : IRequest<PaymentDto>
    {
        public Guid PaymentId { get; set; }

        public GetPaymentByIdQuery(Guid paymentId)
        {
            PaymentId = paymentId;
        }
    }
}
