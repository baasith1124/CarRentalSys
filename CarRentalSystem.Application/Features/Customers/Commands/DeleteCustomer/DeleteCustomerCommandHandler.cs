using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
    {
        private readonly ICustomerRepository _customerRepository;

        public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"DeleteCustomerCommandHandler: Handling delete request for customer {request.CustomerId}");
                
                var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId, cancellationToken);
                if (customer == null)
                {
                    Console.WriteLine($"DeleteCustomerCommandHandler: Customer {request.CustomerId} not found");
                    return false;
                }

                Console.WriteLine($"DeleteCustomerCommandHandler: Found customer {request.CustomerId}, proceeding with deletion");
                var result = await _customerRepository.DeleteCustomerAsync(request.CustomerId, cancellationToken);
                Console.WriteLine($"DeleteCustomerCommandHandler: Deletion result: {result}");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteCustomerCommandHandler: Error deleting customer {request.CustomerId}: {ex.Message}");
                Console.WriteLine($"DeleteCustomerCommandHandler: Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
