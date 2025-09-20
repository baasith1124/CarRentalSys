using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.DeleteCar
{
    public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, bool>
    {
        private readonly ICarRepository _carRepository;
        private readonly IBookingRepository _bookingRepository;

        public DeleteCarCommandHandler(ICarRepository carRepository, IBookingRepository bookingRepository)
        {
            _carRepository = carRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<bool> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
        {
            var car = await _carRepository.GetCarByIdAsync(request.CarId, cancellationToken);
            if (car == null)
                return false;

            try
            {
                // First, get all bookings for this car
                var allBookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);
                var carBookings = allBookings.Where(b => b.CarId == request.CarId).ToList();

                // Cancel all active bookings for this car
                foreach (var booking in carBookings)
                {
                    try
                    {
                        await _bookingRepository.CancelBookingAsync(booking.BookingId, cancellationToken);
                        Console.WriteLine($"Cancelled booking {booking.BookingId} for car {request.CarId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error cancelling booking {booking.BookingId}: {ex.Message}");
                        // Continue with other bookings even if one fails
                    }
                }

                // Now delete the car
                await _carRepository.DeleteCarAsync(request.CarId, cancellationToken);
                Console.WriteLine($"Successfully deleted car {request.CarId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting car {request.CarId}: {ex.Message}");
                throw new Exception($"Failed to delete car: {ex.Message}", ex);
            }
        }
    }
}
