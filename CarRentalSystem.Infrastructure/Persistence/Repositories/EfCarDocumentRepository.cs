using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Repositories
{
    internal class EfCarDocumentRepository : ICarDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public EfCarDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> UploadCarDocumentAsync(CarDocument document, CancellationToken cancellationToken)
        {
            await _context.CarDocuments.AddAsync(document, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return document.DocumentId;
        }

        public async Task<List<CarDocument>> GetDocumentsByCarIdAsync(Guid carId, CancellationToken cancellationToken)
        {
            return await _context.CarDocuments
                .Where(d => d.CarId == carId)
                .ToListAsync(cancellationToken);
        }

        public async Task<CarDocument?> GetDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken)
        {
            return await _context.CarDocuments
                .FirstOrDefaultAsync(d => d.DocumentId == documentId, cancellationToken);
        }

        public async Task<bool> DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken)
        {
            var doc = await _context.CarDocuments.FindAsync(new object[] { documentId }, cancellationToken);
            if (doc == null) return false;

            _context.CarDocuments.Remove(doc);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
