using CarRentalSystem.Application.Common.Interfaces;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.UpdateCar
{
    public class UpdateCarCommandHandler : IRequestHandler<UpdateCarCommand, bool>
    {
        private readonly ICarRepository _carRepository;

        public UpdateCarCommandHandler(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<bool> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
        {
            var car = await _carRepository.GetCarByIdAsync(request.CarId, cancellationToken);
            if (car == null) return false;

            // Update all car properties
            car.Name = request.Name;
            car.Model = request.Model;
            car.ImagePath = request.ImagePath;
            car.AvailableFrom = request.AvailableFrom;
            car.AvailableTo = request.AvailableTo;
            car.CarApprovalStatusId = request.CarApprovalStatusId;
            car.Description = request.Description;
            car.Features = request.Features;
            car.Year = request.Year;
            car.Transmission = request.Transmission;
            car.FuelType = request.FuelType;
            car.RatePerDay = request.RatePerDay;

            // You can handle document changes or file saving via a separate service if needed

            await _carRepository.UpdateCarAsync(car, cancellationToken);
            return true;
        }
    }
}
