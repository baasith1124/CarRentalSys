using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Commands.VerifyKYC
{
    public class VerifyKYCCommandHandler : IRequestHandler<VerifyKYCCommand, bool>
    {
        private readonly IKYCRepository _kycRepository;

        public VerifyKYCCommandHandler(IKYCRepository kycRepository)
        {
            _kycRepository = kycRepository;
        }

        public async Task<bool> Handle(VerifyKYCCommand request, CancellationToken cancellationToken)
        {
            var kyc = await _kycRepository.GetKYCByIdAsync(request.KYCId, cancellationToken);

            if (kyc == null)
                return false;

            return await _kycRepository.UpdateKYCStatusAsync(request.KYCId, request.NewStatus, cancellationToken);
        }
        
    }
}
