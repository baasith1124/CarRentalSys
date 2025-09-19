﻿﻿﻿﻿﻿﻿﻿﻿﻿using CarRentalSystem.Application.Common.Interfaces;
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
            if (request.PickupDate < DateTime.UtcNow)
                throw new ArgumentException("Pickup date cannot be in the past.");

            if (request.PickupDate >= request.ReturnDate)
                throw new ArgumentException("Return date must be after pickup date.");

            if ((request.ReturnDate - request.PickupDate).TotalMinutes < 30)
                throw new ArgumentException("Booking must be at least 30 minutes long.");

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
                CreatedAt = DateTime.UtcNow
            };

            return await _bookingRepository.CreateBookingAsync(booking, cancellationToken);
        }
    }
}
