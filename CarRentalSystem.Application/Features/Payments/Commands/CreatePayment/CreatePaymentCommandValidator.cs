using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Payments.Commands.CreatePayment
{
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty()
                .WithMessage("Booking ID is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Payment amount must be greater than zero.");

            RuleFor(x => x.Method)
                .NotEmpty()
                .WithMessage("Payment method is required.")
                .Must(method =>
                    Enum.TryParse<Domain.Enums.PaymentMethod>(method, true, out _))
                .WithMessage("Invalid payment method.");

            RuleFor(x => x.StripeTxnId)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.StripeTxnId))
                .WithMessage("Transaction ID cannot exceed 100 characters.");
        }
    }
}
