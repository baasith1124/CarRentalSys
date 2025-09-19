using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarApproval.Commands.ApproveOrRejectCar
{
    public class ApproveOrRejectCarCommandValidator : AbstractValidator<ApproveOrRejectCarCommand>
    {
        public ApproveOrRejectCarCommandValidator()
        {
            RuleFor(x => x.CarId)
                .NotEmpty().WithMessage("Car ID is required.");

            RuleFor(x => x.NewStatusId)
                .NotEmpty().WithMessage("Approval status is required.");
        }
    }
}
