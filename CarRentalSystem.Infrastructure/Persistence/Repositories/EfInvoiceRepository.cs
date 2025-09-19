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
    public class EfInvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public EfInvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            invoice.InvoiceId = Guid.NewGuid();
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(cancellationToken);
            return invoice.InvoiceId;
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Include(i => i.Payment)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId, cancellationToken);
        }

        public async Task<Invoice?> GetInvoiceByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Include(i => i.Payment)
                .FirstOrDefaultAsync(i => i.Payment.BookingId == bookingId, cancellationToken);
        }

        public async Task<List<Invoice>> GetAllInvoicesAsync(CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Include(i => i.Payment)
                .ToListAsync(cancellationToken);
        }
        public async Task<Invoice?> GetInvoiceByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.PaymentId == paymentId, cancellationToken);
        }

        public async Task<bool> DeleteInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync(new object[] { invoiceId }, cancellationToken);
            if (invoice == null)
                return false;

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
