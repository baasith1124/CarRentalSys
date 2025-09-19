using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarDocuments.Commands.UploadCarDocument
{
    public class UploadCarDocumentCommandValidator : AbstractValidator<UploadCarDocumentCommand>
    {
        public UploadCarDocumentCommandValidator()
        {
            RuleFor(x => x.CarId).NotEmpty();
            RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(50);
            RuleFor(x => x.FilePath).NotEmpty();
        }
    }
}
