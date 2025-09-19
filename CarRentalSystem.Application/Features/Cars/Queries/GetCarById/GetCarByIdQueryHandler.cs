using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Car;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Queries.GetCarById
{
    public class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, CarDto?>
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;

        public GetCarByIdQueryHandler(ICarRepository carRepository, IMapper mapper)
        {
            _carRepository = carRepository;
            _mapper = mapper;
        }

        public async Task<CarDto?> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
        {
            var car = await _carRepository.GetCarByIdAsync(request.CarId, cancellationToken);

            if (car == null)
                return null;

            return _mapper.Map<CarDto>(car);
        }
    }
}
