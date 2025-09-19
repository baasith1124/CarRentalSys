using CarRentalSystem.Application.Contracts.Car;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.RegisterCar
{
    public class RegisterCarCommand : IRequest<Guid>  // returns CarId
    {
        public string Name { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? ImagePath { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public Guid OwnerId { get; set; }
        public List<IFormFile> Documents { get; set; } = new();
        public string Description { get; set; } = null!;
        public string Features { get; set; } = null!;
        public int Year { get; set; }
        public string Transmission { get; set; } = null!;
        public string FuelType { get; set; } = null!;
        public decimal RatePerDay { get; set; }
    }
}
