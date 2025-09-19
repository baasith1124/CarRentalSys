using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarDocuments.Commands.DeleteCarDocument
{
    public class DeleteCarDocumentCommand : IRequest<bool>
    {
        public Guid DocumentId { get; set; }

        public DeleteCarDocumentCommand(Guid documentId)
        {
            DocumentId = documentId;
        }
    }
}
