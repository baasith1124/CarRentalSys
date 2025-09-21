using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IPaymentRepository
    {
        // Create
        Task<Guid> CreatePaymentAsync(Payment payment, CancellationToken cancellationToken);

        // Read
        Task<Payment?> GetPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken);
        Task<List<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken);
        Task<List<Payment>> GetAllPaymentsAsync(CancellationToken cancellationToken);
        Task<List<Payment>> GetPaymentsByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);

        // Invoice
        Task<Invoice?> GetInvoiceByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken);

        // Refunds 
        Task<bool> ProcessRefundAsync(Guid paymentId, decimal refundAmount, CancellationToken cancellationToken);
    }
}
