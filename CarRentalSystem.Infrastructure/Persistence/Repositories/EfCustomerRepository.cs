using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Repositories
{
    internal class EfCustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public EfCustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken)
        {
            return await _context.Customers
                .Include(c => c.Bookings)
                .Include(c => c.KYCUploads)
                .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
        }

        public async Task<List<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken)
        {
            return await _context.Customers
                .Include(c => c.Bookings)
                .Include(c => c.KYCUploads)
                .ToListAsync(cancellationToken);
        }

        public async Task<Guid> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            await _context.Customers.AddAsync(customer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return customer.Id;
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            _context.Customers.Update(customer);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"EfCustomerRepository: Attempting to delete customer {customerId}");
                
                // Load customer with related data to ensure proper cascade deletion
                var customer = await _context.Customers
                    .Include(c => c.Bookings)
                    .Include(c => c.KYCUploads)
                    .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
                
                if (customer == null)
                {
                    Console.WriteLine($"EfCustomerRepository: Customer {customerId} not found in database");
                    return false;
                }

                Console.WriteLine($"EfCustomerRepository: Found customer {customerId} with {customer.Bookings?.Count ?? 0} bookings and {customer.KYCUploads?.Count ?? 0} KYC uploads");
                
                // Remove the customer - EF Core will handle cascade deletes based on the model configuration
                _context.Customers.Remove(customer);
                
                var result = await _context.SaveChangesAsync(cancellationToken);
                Console.WriteLine($"EfCustomerRepository: Save changes result: {result}");
                
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EfCustomerRepository: Error deleting customer {customerId}: {ex.Message}");
                Console.WriteLine($"EfCustomerRepository: Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
