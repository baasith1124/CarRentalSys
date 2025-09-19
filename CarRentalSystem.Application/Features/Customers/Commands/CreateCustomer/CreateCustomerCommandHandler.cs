using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                ProfileImagePath = request.ProfileImagePath,
                NIC = request.NIC,
                Address = request.Address
            };

            return await _customerRepository.CreateCustomerAsync(customer, cancellationToken);
        }
    }
}
