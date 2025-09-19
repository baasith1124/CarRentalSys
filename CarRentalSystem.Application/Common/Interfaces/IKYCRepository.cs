using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IKYCRepository
    {
        Task<Guid> UploadKYCAsync(KYCUpload kyc, CancellationToken cancellationToken);
        Task<List<KYCUpload>> GetKYCByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<KYCUpload?> GetKYCByIdAsync(Guid kycId, CancellationToken cancellationToken);
        Task<bool> UpdateKYCStatusAsync(Guid kycId, string status, CancellationToken cancellationToken);

        Task<List<KYCUpload>> GetPendingKYCsAsync(CancellationToken cancellationToken);
        Task<List<KYCUpload>> GetAllKYCUploadsAsync(CancellationToken cancellationToken);
    }
}
