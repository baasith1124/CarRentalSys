﻿using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IBookingRepository
    {
        // Create
        Task<Guid> CreateBookingAsync(Booking booking, CancellationToken cancellationToken);

        // Read
        Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken);
        Task<List<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken);
        Task<List<Booking>> GetBookingsByCustomerAsync(Guid customerId, CancellationToken cancellationToken);
        Task<List<Booking>> GetBookingsByCarOwnerAsync(Guid carOwnerId, CancellationToken cancellationToken);

        // Update
        Task UpdateBookingStatusAsync(Guid bookingId, Guid bookingStatusId, Guid? paymentStatusId, CancellationToken cancellationToken);
        Task CancelBookingAsync(Guid bookingId, CancellationToken cancellationToken);

        // Status
        Task<Guid> GetBookingStatusIdByNameAsync(string statusName, CancellationToken cancellationToken);
        Task<Guid> GetPaymentStatusIdByNameAsync(string statusName, CancellationToken cancellationToken);

        // Invoice
        Task<Booking?> GetBookingWithPaymentAsync(Guid bookingId, CancellationToken cancellationToken);

        Task<bool> IsCarAvailableAsync(Guid carId, DateTime pickupDate, DateTime returnDate, CancellationToken cancellationToken);
    }
}
