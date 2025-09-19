using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Car;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Queries.GetMyCars
{
    public class GetMyCarsQueryHandler : IRequestHandler<GetMyCarsQuery, List<CarDto>>
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;

        public GetMyCarsQueryHandler(ICarRepository carRepository, IMapper mapper)
        {
            _carRepository = carRepository;
            _mapper = mapper;
        }

        public async Task<List<CarDto>> Handle(GetMyCarsQuery request, CancellationToken cancellationToken)
        {
            var cars = await _carRepository.GetCarsByOwnerAsync(request.OwnerId, cancellationToken);
            return _mapper.Map<List<CarDto>>(cars);
        }
    }
}
