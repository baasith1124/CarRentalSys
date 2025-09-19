using CarRentalSystem.Application.Common.Interfaces;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.DeleteCar
{
    public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, bool>
    {
        private readonly ICarRepository _carRepository;

        public DeleteCarCommandHandler(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<bool> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
        {
            var car = await _carRepository.GetCarByIdAsync(request.CarId, cancellationToken);
            if (car == null)
                return false;

            // If document deletion is handled elsewhere, you can invoke that service here
            await _carRepository.DeleteCarAsync(request.CarId, cancellationToken);
            return true;
        }
    }
}
