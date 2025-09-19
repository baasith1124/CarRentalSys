using CarRentalSystem.Application.Contracts.Customer;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQuery : IRequest<CustomerDto>
    {
        public Guid CustomerId { get; set; }

        public GetCustomerByIdQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
