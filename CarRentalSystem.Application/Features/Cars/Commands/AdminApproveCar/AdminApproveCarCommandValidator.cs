using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.AdminApproveCar
{
    public class AdminApproveCarCommandValidator : AbstractValidator<AdminApproveCarCommand>
    {
        public AdminApproveCarCommandValidator()
        {
            RuleFor(x => x.CarId).NotEmpty();
            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(status => status == "Approved" || status == "Rejected")
                .WithMessage("Status must be 'Approved' or 'Rejected'");
        }
    }
}
