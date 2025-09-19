using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface ICarDocumentRepository
    {
        Task<Guid> UploadCarDocumentAsync(CarDocument document, CancellationToken cancellationToken);
        Task<List<CarDocument>> GetDocumentsByCarIdAsync(Guid carId, CancellationToken cancellationToken);
        Task<CarDocument?> GetDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken);
        Task<bool> DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken);
    }
}
