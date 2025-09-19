using CarRentalSystem.Application.Contracts.Car;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Queries.SearchCars
{
    public class SearchCarsQuery : IRequest<List<CarDto>>
    {
        public string? Brand { get; set; }
        public string? Transmission { get; set; }
        public string? FuelType { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }

        public DateTime PickupDate { get; set; }
        public DateTime DropDate { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
    }
}
