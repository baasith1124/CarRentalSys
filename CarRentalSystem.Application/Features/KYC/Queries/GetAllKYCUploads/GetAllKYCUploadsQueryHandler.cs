using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;

namespace CarRentalSystem.Application.Features.KYC.Queries.GetAllKYCUploads
{
    public class GetAllKYCUploadsQueryHandler : IRequestHandler<GetAllKYCUploadsQuery, List<KYCUpload>>
    {
        private readonly IKYCRepository _kycRepository;

        public GetAllKYCUploadsQueryHandler(IKYCRepository kycRepository)
        {
            _kycRepository = kycRepository;
        }

        public async Task<List<KYCUpload>> Handle(GetAllKYCUploadsQuery request, CancellationToken cancellationToken)
        {
            return await _kycRepository.GetAllKYCUploadsAsync(cancellationToken);
        }
    }
}
