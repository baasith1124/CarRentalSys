using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Commands.AdminApproveOrRejectKYC
{
    public class AdminApproveOrRejectKYCCommandHandler : IRequestHandler<AdminApproveOrRejectKYCCommand, bool>
    {
        private readonly IKYCRepository _kycRepository;

        public AdminApproveOrRejectKYCCommandHandler(IKYCRepository kycRepository)
        {
            _kycRepository = kycRepository;
        }

        public async Task<bool> Handle(AdminApproveOrRejectKYCCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Processing KYC {request.KYCId} with status {request.NewStatus}");
            
            var kyc = await _kycRepository.GetKYCByIdAsync(request.KYCId, cancellationToken);
            if (kyc == null) 
            {
                Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: KYC {request.KYCId} not found");
                return false;
            }

            Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Found KYC with current status: {kyc.Status}");

            // Optional: check if already approved/rejected
            if (kyc.Status == "Approved" || kyc.Status == "Rejected")
            {
                Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: KYC {request.KYCId} already processed with status {kyc.Status}");
                return false;
            }

            // Update status
            Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Updating KYC {request.KYCId} to status {request.NewStatus}");
            var result = await _kycRepository.UpdateKYCStatusAsync(request.KYCId, request.NewStatus, cancellationToken);
            Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Update result: {result}");

            //  store remarks in a separate table if designed (not implemented here)
            return result;
        }
    }
}
