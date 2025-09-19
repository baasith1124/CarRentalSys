using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Commands.AdminApproveOrRejectKYC
{
    public class AdminApproveOrRejectKYCCommandValidator : AbstractValidator<AdminApproveOrRejectKYCCommand>
    {
        public AdminApproveOrRejectKYCCommandValidator()
        {
            RuleFor(x => x.KYCId).NotEmpty().WithMessage("KYC ID is required.");

            RuleFor(x => x.NewStatus)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => status == "Approved" || status == "Rejected")
                .WithMessage("Status must be either 'Approved' or 'Rejected'.");
        }
    }
}
