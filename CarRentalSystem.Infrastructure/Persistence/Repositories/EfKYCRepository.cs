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
    public class EfKYCRepository : IKYCRepository
    {
        private readonly ApplicationDbContext _context;

        public EfKYCRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> UploadKYCAsync(KYCUpload kyc, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"EfKYCRepository: Adding KYC record for user {kyc.UserId}, type {kyc.DocumentType}, path {kyc.FilePath}");
                await _context.KYCUploads.AddAsync(kyc, cancellationToken);
                Console.WriteLine("EfKYCRepository: KYC record added to context");
                
                var saveResult = await _context.SaveChangesAsync(cancellationToken);
                Console.WriteLine($"EfKYCRepository: Save changes result: {saveResult}");
                Console.WriteLine($"EfKYCRepository: KYC record saved with ID: {kyc.KYCId}");
                
                return kyc.KYCId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EfKYCRepository: Error saving KYC record: {ex.Message}");
                Console.WriteLine($"EfKYCRepository: Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<KYCUpload>> GetKYCByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.KYCUploads
                .Where(k => k.UserId == userId)
                .OrderByDescending(k => k.UploadedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<KYCUpload?> GetKYCByIdAsync(Guid kycId, CancellationToken cancellationToken)
        {
            return await _context.KYCUploads
                .FirstOrDefaultAsync(k => k.KYCId == kycId, cancellationToken);
        }

        public async Task<bool> UpdateKYCStatusAsync(Guid kycId, string status, CancellationToken cancellationToken)
        {
            Console.WriteLine($"EfKYCRepository: Updating KYC {kycId} to status {status}");
            
            var kyc = await _context.KYCUploads
                .FirstOrDefaultAsync(k => k.KYCId == kycId, cancellationToken);

            if (kyc == null)
            {
                Console.WriteLine($"EfKYCRepository: KYC {kycId} not found in database");
                return false;
            }

            Console.WriteLine($"EfKYCRepository: Found KYC {kycId}, current status: {kyc.Status}");
            
            kyc.Status = status;
            _context.KYCUploads.Update(kyc);
            
            var saveResult = await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"EfKYCRepository: Save changes result: {saveResult}");

            return true;
        }
        public async Task<List<KYCUpload>> GetPendingKYCsAsync(CancellationToken cancellationToken)
        {
            return await _context.KYCUploads
                .Where(k => k.Status == "Pending")
                .ToListAsync(cancellationToken);
        }

        public async Task<List<KYCUpload>> GetAllKYCUploadsAsync(CancellationToken cancellationToken)
        {
            return await _context.KYCUploads
                .OrderByDescending(k => k.UploadedAt)
                .ToListAsync(cancellationToken);
        }

    }
}
