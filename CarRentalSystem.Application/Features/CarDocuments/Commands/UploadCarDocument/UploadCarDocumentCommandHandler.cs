using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarDocuments.Commands.UploadCarDocument
{
    public class UploadCarDocumentCommandHandler : IRequestHandler<UploadCarDocumentCommand, Guid>
    {
        private readonly ICarDocumentRepository _documentRepository;

        public UploadCarDocumentCommandHandler(ICarDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<Guid> Handle(UploadCarDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = new CarDocument
            {
                CarId = request.CarId,
                DocumentType = request.DocumentType,
                FilePath = request.FilePath,
                UploadedAt = DateTime.UtcNow
            };

            return await _documentRepository.UploadCarDocumentAsync(document, cancellationToken);
        }
    }
}
