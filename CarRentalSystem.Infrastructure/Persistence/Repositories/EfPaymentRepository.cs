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
    public class EfPaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public EfPaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreatePaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            payment.PaymentId = Guid.NewGuid();
            await _context.Payments.AddAsync(payment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return payment.PaymentId;
        }

        public async Task<Payment?> GetPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId, cancellationToken);
        }

        public async Task<List<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken)
        {
            return await _context.Payments
                .Where(p => p.BookingId == bookingId)
                .Include(p => p.Invoice)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Payment>> GetAllPaymentsAsync(CancellationToken cancellationToken)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                .Include(p => p.Invoice)
                .ToListAsync(cancellationToken);
        }

        public async Task<Invoice?> GetInvoiceByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.PaymentId == paymentId, cancellationToken);
        }

        public async Task<bool> ProcessRefundAsync(Guid paymentId, decimal refundAmount, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments.FindAsync(new object[] { paymentId }, cancellationToken);
            if (payment == null || refundAmount <= 0 || refundAmount > payment.Amount)
                return false;

            // Simulated refund logic
            payment.Amount -= refundAmount;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
