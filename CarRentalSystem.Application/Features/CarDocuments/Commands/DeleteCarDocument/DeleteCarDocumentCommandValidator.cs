using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarDocuments.Commands.DeleteCarDocument
{
    public class DeleteCarDocumentCommandValidator : AbstractValidator<DeleteCarDocumentCommand>
    {
        public DeleteCarDocumentCommandValidator()
        {
            RuleFor(x => x.DocumentId).NotEmpty().WithMessage("Document ID is required.");
        }
    }
}
