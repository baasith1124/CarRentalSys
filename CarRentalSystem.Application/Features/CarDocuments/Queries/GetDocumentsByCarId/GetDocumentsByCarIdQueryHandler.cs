using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Application.Contracts.CarDocument;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarDocuments.Queries.GetDocumentsByCarId
{
    public class GetDocumentsByCarIdQueryHandler : IRequestHandler<GetDocumentsByCarIdQuery, List<CarDocumentDto>>
    {
        private readonly ICarDocumentRepository _repository;
        private readonly IMapper _mapper;

        public GetDocumentsByCarIdQueryHandler(ICarDocumentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CarDocumentDto>> Handle(GetDocumentsByCarIdQuery request, CancellationToken cancellationToken)
        {
            var docs = await _repository.GetDocumentsByCarIdAsync(request.CarId, cancellationToken);
            return _mapper.Map<List<CarDocumentDto>>(docs);
        }
    }
}
