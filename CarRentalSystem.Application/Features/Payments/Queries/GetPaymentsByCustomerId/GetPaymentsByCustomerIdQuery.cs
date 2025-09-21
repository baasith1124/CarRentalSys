using CarRentalSystem.Application.Contracts.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Payments.Queries.GetPaymentsByCustomerId
{
    public class GetPaymentsByCustomerIdQuery : IRequest<List<PaymentDto>>
    {
        public Guid CustomerId { get; set; }

        public GetPaymentsByCustomerIdQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
