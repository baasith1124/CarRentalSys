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
        public RegisterCarValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Model).NotEmpty().MaximumLength(50);
            RuleFor(x => x.AvailableFrom).LessThan(x => x.AvailableTo);
            RuleFor(x => x.OwnerId).NotEmpty();
        }
    }
}
