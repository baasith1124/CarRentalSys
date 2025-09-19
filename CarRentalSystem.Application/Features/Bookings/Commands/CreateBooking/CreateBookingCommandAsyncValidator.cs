using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Common.Validators;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandAsyncValidator : AbstractValidator<CreateBookingCommand>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ILocalizationService _localizationService;

        public CreateBookingCommandAsyncValidator(
            IBookingRepository bookingRepository,
            ILocalizationService localizationService)
        {
            _bookingRepository = bookingRepository;
            _localizationService = localizationService;

            // Async validation for car availability
            RuleFor(x => x.CarId)
                .MustAsync(async (command, carId, cancellation) =>
                {
                    if (carId == Guid.Empty)
                        return true; // Let the required validator handle empty GUID

                    return await _bookingRepository.IsCarAvailableAsync(
                        carId, 
                        command.PickupDate, 
                        command.ReturnDate, 
                        cancellation);
                })
                .WithMessage(_localizationService.GetString("CarNotAvailable"))
                .When(x => x.CarId != Guid.Empty && x.PickupDate != default && x.ReturnDate != default);
        }
    }
}
