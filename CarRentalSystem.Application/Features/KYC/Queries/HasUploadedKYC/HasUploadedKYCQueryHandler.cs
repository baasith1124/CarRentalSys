using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Queries.HasUploadedKYC
{
    public class HasUploadedKYCQueryHandler : IRequestHandler<HasUploadedKYCQuery, bool>
    {
        private readonly IKYCRepository _kycRepository;

        public HasUploadedKYCQueryHandler(IKYCRepository kycRepository)
        {
            _kycRepository = kycRepository;
        }

        public async Task<bool> Handle(HasUploadedKYCQuery request, CancellationToken cancellationToken)
        {
            var kycs = await _kycRepository.GetKYCByUserIdAsync(request.UserId, cancellationToken);
            return kycs != null && kycs.Any();
        }
    }
}
