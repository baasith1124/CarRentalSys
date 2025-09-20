using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.RegisterCar
{
    public class RegisterCarCommandHandler : IRequestHandler<RegisterCarCommand, Guid>
    {
        private readonly ICarRepository _carRepository;

        public RegisterCarCommandHandler(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<Guid> Handle(RegisterCarCommand request, CancellationToken cancellationToken)
        {
            // Use resilient lookup that creates the status if missing
            var pendingStatusId = await _carRepository.GetStatusIdByNameAsync("Pending", cancellationToken);

            var car = new Car
            {
                CarId = Guid.NewGuid(),
                Name = request.Name,
                Model = request.Model,
                ImagePath = request.ImagePath,
                Description = request.Description,
                Features = request.Features,
                Year = request.Year,
                Transmission = request.Transmission,
                FuelType = request.FuelType,
                RatePerDay = request.RatePerDay,
                AvailableFrom = request.AvailableFrom,
                AvailableTo = request.AvailableTo,
                OwnerId = request.OwnerId,
                CarApprovalStatusId = pendingStatusId
            };

            // Debug: Log the car being created
            Console.WriteLine($"Creating car: {car.Name} {car.Model} for owner {car.OwnerId}");

            var carId = await _carRepository.AddCarAsync(car, cancellationToken);
            
            // Debug: Log successful creation
            Console.WriteLine($"Car created successfully with ID: {carId}");
            
            return carId;
        }
    }
}
