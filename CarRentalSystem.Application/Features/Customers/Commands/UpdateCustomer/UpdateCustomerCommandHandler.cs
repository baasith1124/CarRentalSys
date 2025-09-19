using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, bool>
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId, cancellationToken);

            if (customer == null)
                return false;

            customer.FullName = request.FullName;
            customer.Email = request.Email;
            customer.ProfileImagePath = request.ProfileImagePath;
            customer.NIC = request.NIC;
            customer.Address = request.Address;

            return await _customerRepository.UpdateCustomerAsync(customer, cancellationToken);
        }
    }
}
