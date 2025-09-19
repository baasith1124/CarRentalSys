using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Invoices.Commands.CreateInvoice
{
    public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceCommandValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEmpty().WithMessage("Payment ID is required.");

            RuleFor(x => x.FilePath)
                .NotEmpty().WithMessage("File path is required.")
                .MaximumLength(255).WithMessage("File path must be less than 255 characters.");
        }
    }
}
