using CarRentalSystem.Application.Contracts.Car;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Queries.GetMyCars
{
    public class GetMyCarsQuery : IRequest<List<CarDto>>
    {
        public Guid OwnerId { get; set; }

        public GetMyCarsQuery(Guid ownerId)
        {
            OwnerId = ownerId;
        }
    }
}
