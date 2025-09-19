using CarRentalSystem.Application.Common.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.UpdateCar
{
    public class UpdateCarCommandValidator : AbstractValidator<UpdateCarCommand>
    {
        private readonly ILocalizationService _localizationService;

        public UpdateCarCommandValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.CarId)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("CarIdRequired"));

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("CarNameRequired"))
                .MaximumLength(100)
                .WithMessage(_localizationService.GetString("CarNameMaxLength", 100));

            RuleFor(x => x.Model)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("CarModelRequired"))
                .MaximumLength(100)
                .WithMessage(_localizationService.GetString("CarModelMaxLength", 100));

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

            RuleFor(x => x.CarApprovalStatusId)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("CarApprovalStatusIdRequired"));
        }
    }
}
