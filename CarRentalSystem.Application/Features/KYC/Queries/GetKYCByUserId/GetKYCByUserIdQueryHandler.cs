using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.KYC;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Queries.GetKYCByUserId
{
    public class GetKYCByUserIdQueryHandler : IRequestHandler<GetKYCByUserIdQuery, List<KYCDto>>
    {
        private readonly IKYCRepository _kycRepository;
        private readonly IMapper _mapper;

        public GetKYCByUserIdQueryHandler(IKYCRepository kycRepository, IMapper mapper)
        {
            _kycRepository = kycRepository;
            _mapper = mapper;
        }

        public async Task<List<KYCDto>> Handle(GetKYCByUserIdQuery request, CancellationToken cancellationToken)
        {
            var kycUploads = await _kycRepository.GetKYCByUserIdAsync(request.UserId, cancellationToken);
            return _mapper.Map<List<KYCDto>>(kycUploads);
        }
    }
}
