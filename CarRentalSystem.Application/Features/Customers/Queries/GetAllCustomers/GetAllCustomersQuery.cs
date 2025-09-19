using CarRentalSystem.Application.Contracts.Customer;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQuery : IRequest<List<CustomerDto>>
    {
    }
}
