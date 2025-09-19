using CarRentalSystem.Application.Common.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.RegisterCar
{
    public class RegisterCarValidator : AbstractValidator<RegisterCarCommand>
    {
        private readonly ILocalizationService _localizationService;

        public RegisterCarValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("CarNameRequired"))
                .MaximumLength(100)
                .WithMessage(_localizationService.GetString("CarNameMaxLength", 100));

            RuleFor(x => x.Model)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("CarModelRequired"))
                .MaximumLength(50)
                .WithMessage(_localizationService.GetString("CarModelMaxLength", 50));

            RuleFor(x => x.Year)
                .GreaterThan(1900)
                .WithMessage(_localizationService.GetString("YearMin", 1900))
                .LessThanOrEqualTo(DateTime.Now.Year + 1)
                .WithMessage(_localizationService.GetString("YearMax"));

            RuleFor(x => x.RatePerDay)
                .GreaterThan(0)
                .WithMessage(_localizationService.GetString("RatePerDayMin"));

            RuleFor(x => x.AvailableFrom)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("AvailableFromRequired"))
                .LessThan(x => x.AvailableTo)
                .WithMessage(_localizationService.GetString("DateFromBeforeTo"));

            RuleFor(x => x.AvailableTo)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("AvailableToRequired"))
                .GreaterThan(x => x.AvailableFrom)
                .WithMessage(_localizationService.GetString("DateToAfterFrom"));

            RuleFor(x => x.OwnerId)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("OwnerIdRequired"));

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage(_localizationService.GetString("DescriptionMaxLength", 500))
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Features)
                .MaximumLength(1000)
                .WithMessage(_localizationService.GetString("FeaturesMaxLength", 1000))
                .When(x => !string.IsNullOrWhiteSpace(x.Features));
        }
    }
}
