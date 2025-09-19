using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Car;

using MediatR;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Queries.GetAllCars
{
    public class GetAllCarsQueryHandler : IRequestHandler<GetAllCarsQuery, List<CarDto>>
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;

        public GetAllCarsQueryHandler(ICarRepository carRepository, IMapper mapper)
        {
            _carRepository = carRepository;
            _mapper = mapper;
        }

        public async Task<List<CarDto>> Handle(GetAllCarsQuery request, CancellationToken cancellationToken)
        {
            var cars = await _carRepository.GetAllCarsWithStatusAsync(cancellationToken);
            return _mapper.Map<List<CarDto>>(cars);
        }
    }
}
