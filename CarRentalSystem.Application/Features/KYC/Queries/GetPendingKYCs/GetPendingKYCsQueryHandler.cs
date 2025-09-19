using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.KYC;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Queries.GetPendingKYCs
{
    public class GetPendingKYCsQueryHandler : IRequestHandler<GetPendingKYCsQuery, List<KYCDto>>
    {
        private readonly IKYCRepository _kycRepository;
        private readonly IMapper _mapper;

        public GetPendingKYCsQueryHandler(IKYCRepository kycRepository, IMapper mapper)
        {
            _kycRepository = kycRepository;
            _mapper = mapper;
        }

        public async Task<List<KYCDto>> Handle(GetPendingKYCsQuery request, CancellationToken cancellationToken)
        {
            var pendingKycs = await _kycRepository.GetPendingKYCsAsync(cancellationToken);
            return _mapper.Map<List<KYCDto>>(pendingKycs);
        }
    }
}
