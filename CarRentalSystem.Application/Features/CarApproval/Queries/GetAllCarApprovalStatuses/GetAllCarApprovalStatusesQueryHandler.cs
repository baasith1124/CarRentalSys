using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.CarApprovalStatus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarApproval.Queries.GetAllCarApprovalStatuses
{
    public class GetAllCarApprovalStatusesQueryHandler : IRequestHandler<GetAllCarApprovalStatusesQuery, List<CarApprovalStatusDto>>
    {
        private readonly ICarApprovalStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetAllCarApprovalStatusesQueryHandler(ICarApprovalStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CarApprovalStatusDto>> Handle(GetAllCarApprovalStatusesQuery request, CancellationToken cancellationToken)
        {
            var statuses = await _repository.GetAllStatusesAsync(cancellationToken);
            return _mapper.Map<List<CarApprovalStatusDto>>(statuses);
        }
    }
}
