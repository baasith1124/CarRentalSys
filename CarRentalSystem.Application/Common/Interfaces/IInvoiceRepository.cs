using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IInvoiceRepository
    {
        // Create
        Task<Guid> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);

        // Read
        Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken);
        Task<Invoice?> GetInvoiceByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken);
        Task<List<Invoice>> GetAllInvoicesAsync(CancellationToken cancellationToken);
        Task<Invoice?> GetInvoiceByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken);


        // Delete (if needed)
        Task<bool> DeleteInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken);

    }
}
