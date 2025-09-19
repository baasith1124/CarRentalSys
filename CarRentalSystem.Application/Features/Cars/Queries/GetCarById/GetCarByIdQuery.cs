using CarRentalSystem.Application.Contracts.Car;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Queries.GetCarById
{
    public class GetCarByIdQuery : IRequest<CarDto?>
    {
        public Guid CarId { get; set; }

        public GetCarByIdQuery(Guid carId)
        {
            CarId = carId;
        }
    }
}
