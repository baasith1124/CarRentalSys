using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarDocuments.Commands.DeleteCarDocument
{
    public class DeleteCarDocumentCommandHandler : IRequestHandler<DeleteCarDocumentCommand, bool>
    {
        private readonly ICarDocumentRepository _repository;

        public DeleteCarDocumentCommandHandler(ICarDocumentRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteCarDocumentCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteDocumentAsync(request.DocumentId, cancellationToken);
        }
    }
}
