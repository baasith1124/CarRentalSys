using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarDocuments.Commands.UploadCarDocument
{
    public class UploadCarDocumentCommand : IRequest<Guid>
    {
        public Guid CarId { get; set; }
        public string DocumentType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
    }
}
