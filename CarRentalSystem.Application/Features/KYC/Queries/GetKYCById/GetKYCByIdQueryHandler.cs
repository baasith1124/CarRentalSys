using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.KYC;
using MediatR;

namespace CarRentalSystem.Application.Features.KYC.Queries.GetKYCById
{
    public class GetKYCByIdQueryHandler : IRequestHandler<GetKYCByIdQuery, KYCUploadDto?>
    {
        private readonly IKYCRepository _kycRepository;
        private readonly IMapper _mapper;

        public GetKYCByIdQueryHandler(IKYCRepository kycRepository, IMapper mapper)
        {
            _kycRepository = kycRepository;
            _mapper = mapper;
        }

        public async Task<KYCUploadDto?> Handle(GetKYCByIdQuery request, CancellationToken cancellationToken)
        {
            var kyc = await _kycRepository.GetKYCByIdAsync(request.KYCId, cancellationToken);
            return kyc != null ? _mapper.Map<KYCUploadDto>(kyc) : null;
        }
    }
}
