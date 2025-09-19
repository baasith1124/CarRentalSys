using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Notifications.Commands.SendNotification
{
    public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
    {
        public SendNotificationCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message content cannot be empty.")
                .MaximumLength(500).WithMessage("Message is too long.");

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title is too long.")
                .When(x => !string.IsNullOrWhiteSpace(x.Title));
        }
    }
}
