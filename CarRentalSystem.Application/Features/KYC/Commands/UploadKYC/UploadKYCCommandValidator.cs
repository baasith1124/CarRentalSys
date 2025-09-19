using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Command.UploadKYC
{
    public class UploadKYCCommandValidator : AbstractValidator<UploadKYCCommand>
    {
        public UploadKYCCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.DocumentType)
                .NotEmpty().WithMessage("Document type is required.")
                .MaximumLength(50).WithMessage("Document type too long.");

            RuleFor(x => x.FilePath)
                .NotEmpty().WithMessage("File path is required.");
        }
    }
}
