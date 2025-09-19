using CarRentalSystem.Application.Common.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        private readonly ILocalizationService _localizationService;

        public CreateBookingCommandValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.CarId)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("CarIdRequired"));

            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("CustomerIdRequired"));

            RuleFor(x => x.PickupDate)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("PickupDateRequired"))
                .GreaterThan(DateTime.UtcNow)
                .WithMessage(_localizationService.GetString("PickupDatePast"))
                .LessThan(x => x.ReturnDate)
                .WithMessage(_localizationService.GetString("PickupDateBeforeReturn"));

            RuleFor(x => x.ReturnDate)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("ReturnDateRequired"))
                .GreaterThan(x => x.PickupDate)
                .WithMessage(_localizationService.GetString("ReturnDateAfterPickup"))
                .Must((request, returnDate) => (returnDate - request.PickupDate).TotalMinutes >= 30)
                .WithMessage(_localizationService.GetString("BookingMinDuration"));

            RuleFor(x => x.TotalCost)
                .GreaterThan(0)
                .WithMessage(_localizationService.GetString("TotalCostMin"));
        }
    }
}
