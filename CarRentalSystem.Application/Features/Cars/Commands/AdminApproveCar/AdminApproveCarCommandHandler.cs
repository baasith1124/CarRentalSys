using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.AdminApproveCar
{
    public class AdminApproveCarCommandHandler : IRequestHandler<AdminApproveCarCommand, bool>
    {
        private readonly ICarRepository _carRepository;

        public AdminApproveCarCommandHandler(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<bool> Handle(AdminApproveCarCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"AdminApproveCarCommand - CarId: {request.CarId}, Status: '{request.Status}', IsApproved: {request.IsApproved}");
            
            var car = await _carRepository.GetCarByIdAsync(request.CarId, cancellationToken);
            if (car == null)
                throw new KeyNotFoundException("Car not found");

            Console.WriteLine($"Found car: {car.Name} {car.Model}, Current status: {car.CarApprovalStatus?.Name}");
            
            var newStatusId = await _carRepository.GetStatusIdByNameAsync(request.Status, cancellationToken);
            if (newStatusId == Guid.Empty)
                throw new Exception($"Invalid status: {request.Status}");

            Console.WriteLine($"New status ID: {newStatusId}");
            
            car.CarApprovalStatusId = newStatusId;
            await _carRepository.UpdateCarAsync(car, cancellationToken);
            
            Console.WriteLine($"Car {car.Name} {car.Model} status updated to {request.Status}");
            return true;
        }
    }
}
