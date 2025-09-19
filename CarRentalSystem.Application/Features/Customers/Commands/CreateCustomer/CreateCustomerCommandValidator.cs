using CarRentalSystem.Application.Common.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        private readonly ILocalizationService _localizationService;

        public CreateCustomerCommandValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("NameRequired"))
                .MaximumLength(100)
                .WithMessage(_localizationService.GetString("NameMaxLength", 100))
                .Matches(@"^[a-zA-Z\s]+$")
                .WithMessage(_localizationService.GetString("NameFormat"));

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(_localizationService.GetString("EmailRequired"))
                .EmailAddress()
                .WithMessage(_localizationService.GetString("EmailInvalid"))
                .MaximumLength(255)
                .WithMessage(_localizationService.GetString("EmailMaxLength", 255));

            RuleFor(x => x.NIC)
                .MaximumLength(20)
                .WithMessage(_localizationService.GetString("NICMaxLength", 20))
                .Matches(@"^[0-9]{9}[vVxX]?$|^[0-9]{12}$")
                .WithMessage(_localizationService.GetString("NICFormat"))
                .When(x => !string.IsNullOrWhiteSpace(x.NIC));

            RuleFor(x => x.Address)
                .MaximumLength(200)
                .WithMessage(_localizationService.GetString("AddressMaxLength", 200))
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.ProfileImagePath)
                .MaximumLength(500)
                .WithMessage(_localizationService.GetString("ProfileImagePathMaxLength", 500))
                .When(x => !string.IsNullOrWhiteSpace(x.ProfileImagePath));
        }
    }
}
