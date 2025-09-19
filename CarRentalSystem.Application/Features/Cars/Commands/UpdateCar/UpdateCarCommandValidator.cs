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
        public UpdateCarCommandValidator()
        {
            RuleFor(x => x.CarId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
            RuleFor(x => x.AvailableFrom).LessThan(x => x.AvailableTo)
                .WithMessage("AvailableFrom must be earlier than AvailableTo.");
        }
    }
}
