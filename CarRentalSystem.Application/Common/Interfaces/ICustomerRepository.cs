using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken);
        Task<List<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken);
        Task<Guid> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken);
        Task<bool> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken);
        Task<bool> DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken);
    }
}
