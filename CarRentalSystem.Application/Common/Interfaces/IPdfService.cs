using CarRentalSystem.Application.Contracts.Booking;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateReceiptPdfAsync(BookingDto booking, string paymentId, decimal amount);
        Task<byte[]> GenerateInvoicePdfAsync(BookingDto booking, string invoiceId);
    }
}
