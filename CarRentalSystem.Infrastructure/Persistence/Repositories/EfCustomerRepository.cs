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
                .Include(c => c.Notifications)
                .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
        }

        public async Task<List<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken)
        {
            return await _context.Customers
                .Include(c => c.Bookings)
                .Include(c => c.KYCUploads)
                .Include(c => c.Notifications)
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
            var customer = await _context.Customers.FindAsync(new object[] { customerId }, cancellationToken);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
