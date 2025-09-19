using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Command.UploadKYC
{
    public class UploadKYCCommandHandler : IRequestHandler<UploadKYCCommand, Guid>
    {
        private readonly IKYCRepository _kycRepository;

        public UploadKYCCommandHandler(IKYCRepository kycRepository)
        {
            _kycRepository = kycRepository;
        }

        public async Task<Guid> Handle(UploadKYCCommand request, CancellationToken cancellationToken)
        {
            var kyc = new KYCUpload
            {
                KYCId = Guid.NewGuid(),
                UserId = request.UserId,
                DocumentType = request.DocumentType,
                FilePath = request.FilePath,
                Status = "Pending",
                UploadedAt = DateTime.UtcNow
            };

            await _kycRepository.UploadKYCAsync(kyc, cancellationToken);

            return kyc.KYCId;
        }
    }
}
