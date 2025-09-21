using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Car;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Queries.SearchCars
{
    public class SearchCarsQueryHandler : IRequestHandler<SearchCarsQuery, List<CarDto>>
    {
        private readonly ICarRepository _carRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public SearchCarsQueryHandler(ICarRepository carRepository, IBookingRepository bookingRepository, IMapper mapper)
        {
            _carRepository = carRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<List<CarDto>> Handle(SearchCarsQuery request, CancellationToken cancellationToken)
        {
            var allApprovedCars = await _carRepository.GetCarsByApprovalStatusAsync("Approved", cancellationToken);

            var filtered = allApprovedCars
                .Where(car =>
                    car.AvailableFrom <= request.PickupDate &&
                    car.AvailableTo >= request.DropDate &&
                    (string.IsNullOrEmpty(request.Brand) || car.Name.Equals(request.Brand, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(request.Transmission) || car.Transmission.Equals(request.Transmission, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(request.FuelType) || car.FuelType.Equals(request.FuelType, StringComparison.OrdinalIgnoreCase)) &&
                    (!request.MinYear.HasValue || car.Year >= request.MinYear) &&
                    (!request.MaxYear.HasValue || car.Year <= request.MaxYear)
                );

            // Filter out cars that are not available for the requested time period
            var availableCars = new List<Domain.Entities.Car>();
            foreach (var car in filtered)
            {
                bool isAvailable = await _bookingRepository.IsCarAvailableAsync(car.CarId, request.PickupDate, request.DropDate, cancellationToken);
                if (isAvailable)
                {
                    availableCars.Add(car);
                }
            }

            var carDtos = _mapper.Map<List<CarDto>>(availableCars);

            // Calculate total rental days
            int totalDays = (int)(request.DropDate - request.PickupDate).TotalDays;
            if (totalDays <= 0) totalDays = 1;

            foreach (var carDto in carDtos)
            {
                decimal deliveryFee = CalculateDeliveryFee(request.PickupLocation, request.DropLocation); // Optional
                carDto.EstimatedCost = (carDto.RatePerDay * totalDays) + deliveryFee;
            }

            return carDtos;
        }
        private decimal CalculateDeliveryFee(string pickup, string drop)
        {
            if (string.Equals(pickup, drop, StringComparison.OrdinalIgnoreCase))
                return 0;

            return 2000m; // Fixed fee or calculate based on distance
        }

    }
}
