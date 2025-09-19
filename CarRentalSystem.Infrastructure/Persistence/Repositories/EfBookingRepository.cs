﻿﻿using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Repositories
{
    public class EfBookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public EfBookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateBookingAsync(Booking booking, CancellationToken cancellationToken)
        {
            await _context.Bookings.AddAsync(booking, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return booking.BookingId;
        }

        public async Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .Include(b => b.PaymentStatus)
                .Include(b => b.BookingStatus)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken);
        }

        public async Task<List<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .Include(b => b.PaymentStatus)
                .Include(b => b.BookingStatus)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Booking>> GetBookingsByCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .Include(b => b.Car)
                .Include(b => b.PaymentStatus)
                .Include(b => b.BookingStatus)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Booking>> GetBookingsByCarOwnerAsync(Guid carOwnerId, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .Where(b => b.Car.OwnerId == carOwnerId)
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .Include(b => b.PaymentStatus)
                .Include(b => b.BookingStatus)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateBookingStatusAsync(Guid bookingId, Guid bookingStatusId, Guid? paymentStatusId, CancellationToken cancellationToken)
        {
            var booking = await _context.Bookings.FindAsync(new object[] { bookingId }, cancellationToken);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            booking.BookingStatusId = bookingStatusId;

            if (paymentStatusId.HasValue)
                booking.PaymentStatusId = paymentStatusId.Value;

            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task CancelBookingAsync(Guid bookingId, CancellationToken cancellationToken)
        {
            var booking = await _context.Bookings.FindAsync(new object[] { bookingId }, cancellationToken);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            var cancelStatusId = await GetBookingStatusIdByNameAsync("Cancelled", cancellationToken);
            booking.BookingStatusId = cancelStatusId;

            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Guid> GetBookingStatusIdByNameAsync(string statusName, CancellationToken cancellationToken)
        {
            var status = await _context.BookingStatuses
                .FirstOrDefaultAsync(s => s.Name == statusName, cancellationToken);

            return status?.Id ?? throw new Exception($"Booking status '{statusName}' not found.");
        }

        public async Task<Guid> GetPaymentStatusIdByNameAsync(string statusName, CancellationToken cancellationToken)
        {
            var status = await _context.PaymentStatuses
                .FirstOrDefaultAsync(s => s.Name == statusName, cancellationToken);

            return status?.Id ?? throw new Exception($"Payment status '{statusName}' not found.");
        }

        public async Task<Booking?> GetBookingWithPaymentAsync(Guid bookingId, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .Include(b => b.Payment)
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken);
        }
        public async Task<bool> IsCarAvailableAsync(Guid carId, DateTime pickupDate, DateTime returnDate, CancellationToken cancellationToken)
        {
            return !await _context.Bookings
                .Include(b => b.BookingStatus)
                .Include(b => b.PaymentStatus)
                .AnyAsync(b =>
                    b.CarId == carId &&
                    // Only consider bookings that are:
                    // 1. Approved (confirmed bookings)
                    // 2. Pending with Paid status (payment completed but not yet approved)
                    (b.BookingStatus.Name == "Approved" || 
                     (b.BookingStatus.Name == "Pending" && b.PaymentStatus.Name == "Paid")) &&
                    // Check for date overlap
                    (
                        (pickupDate < b.ReturnDate && returnDate > b.PickupDate)
                    ),
                    cancellationToken);
        }
    }
}
