﻿using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarRepository _carRepository;

        public CreateBookingCommandHandler(IBookingRepository bookingRepository, ICarRepository carRepository)
        {
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
        }

        public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            // Business logic validation - check car availability
            bool isAvailable = await _bookingRepository.IsCarAvailableAsync(request.CarId, request.PickupDate, request.ReturnDate, cancellationToken);

            if (!isAvailable)
                throw new InvalidOperationException("This car is already booked for the selected period.");

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                CarId = request.CarId,
                CustomerId = request.CustomerId,
                PickupDate = request.PickupDate,
                ReturnDate = request.ReturnDate,
                TotalCost = request.TotalCost,
                BookingStatusId = await _bookingRepository.GetBookingStatusIdByNameAsync("Pending", cancellationToken),
                PaymentStatusId = await _bookingRepository.GetPaymentStatusIdByNameAsync("Unpaid", cancellationToken),
                CreatedAt = DateTime.UtcNow,
                PickupLocation = request.PickupLocation,
                DropLocation = request.DropLocation,
                PickupLatitude = request.PickupLatitude,
                PickupLongitude = request.PickupLongitude,
                DropLatitude = request.DropLatitude,
                DropLongitude = request.DropLongitude
            };

            return await _bookingRepository.CreateBookingAsync(booking, cancellationToken);
        }
    }
}
