using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.DeleteCar
{
    public class DeleteCarCommandValidator : AbstractValidator<DeleteCarCommand>
    {
        public DeleteCarCommandValidator()
        {
            RuleFor(x => x.CarId).NotEmpty().WithMessage("CarId is required");
        }
    }
}
