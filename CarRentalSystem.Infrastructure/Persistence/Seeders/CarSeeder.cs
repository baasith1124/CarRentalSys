using CarRentalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Seeders
{
    public static class CarSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Check if we have any approved cars, if not, seed them
            var hasApprovedCars = await context.Cars
                .Include(c => c.CarApprovalStatus)
                .AnyAsync(c => c.CarApprovalStatus.Name == "Approved");
            
            if (hasApprovedCars) return;

            var defaultOwner = await context.Users
                .Where(u => u.Email == "admin@carrental.com")
                .FirstOrDefaultAsync();

            if (defaultOwner == null)
                throw new Exception("Admin user not found. Ensure IdentitySeeder has run.");

            var defaultOwnerId = defaultOwner.Id;
            var defaultApprovalStatusId = await context.CarApprovalStatuses
                .Where(x => x.Name == "Approved")
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var today = DateTime.UtcNow.Date;

            var cars = new List<Car>
        {
            new()
            {
                CarId = Guid.NewGuid(),
                Name = "Acura",
                Model = "TLX",
                Description = "A luxury sedan with premium features.",
                Features = "Leather seats, Sunroof, Adaptive cruise control",
                Year = 2022,
                FuelType = "Petrol",
                Transmission = "Automatic",
                ImagePath = "acura.png",
                AvailableFrom = today,
                AvailableTo = today.AddDays(120),
                OwnerId = defaultOwnerId,
                RatePerDay = 15000m,
                CarApprovalStatusId = defaultApprovalStatusId
            },
            new()
            {
                CarId = Guid.NewGuid(),
                Name = "Alfa Romeo",
                Model = "Giulia",
                Description = "A sporty Italian sedan with stylish design.",
                Features = "Turbocharged engine, Leather interior, Navigation",
                Year = 2021,
                FuelType = "Petrol",
                Transmission = "Automatic",
                ImagePath = "alfaromeo.png",
                AvailableFrom = today,
                AvailableTo = today.AddDays(120),
                OwnerId = defaultOwnerId,
                RatePerDay = 28000m,
                CarApprovalStatusId = defaultApprovalStatusId
            },
            new()
            {
                CarId = Guid.NewGuid(),
                Name = "Chevrolet",
                Model = "Camaro ZL1",
                Description = "A high-performance American muscle car.",
                Features = "650 HP, Launch Control, Premium Audio",
                Year = 2023,
                FuelType = "Petrol",
                Transmission = "Manual",
                ImagePath = "camaro.png",
                AvailableFrom = today,
                AvailableTo = today.AddDays(120),
                OwnerId = defaultOwnerId,
                RatePerDay = 1600m,
                CarApprovalStatusId = defaultApprovalStatusId
            },
            new()
            {
                CarId = Guid.NewGuid(),
                Name = "Ferrari",
                Model = "488 Spider",
                Description = "An exotic convertible supercar.",
                Features = "V8 Twin Turbo, Carbon ceramic brakes, Paddle shift",
                Year = 2022,
                FuelType = "Petrol",
                Transmission = "Automatic",
                ImagePath = "ferrari.png",
                AvailableFrom = today,
                AvailableTo = today.AddDays(120),
                OwnerId = defaultOwnerId,
                RatePerDay = 50000m,
                CarApprovalStatusId = defaultApprovalStatusId
            },
            new()
            {
                CarId = Guid.NewGuid(),
                Name = "Citroen",
                Model = "C3",
                Description = "A compact hatchback ideal for city driving.",
                Features = "Touchscreen, Rear camera, Bluetooth",
                Year = 2021,
                FuelType = "Petrol",
                Transmission = "Manual",
                ImagePath = "citroen.png",
                AvailableFrom = today,
                AvailableTo = today.AddDays(120),
                OwnerId = defaultOwnerId,
                RatePerDay = 7500m,
                CarApprovalStatusId = defaultApprovalStatusId
            },
            new()
            {
                CarId = Guid.NewGuid(),
                Name = "Fiat",
                Model = "Punto",
                Description = "Affordable hatchback with good mileage.",
                Features = "AC, Airbags, ABS",
                Year = 2020,
                FuelType = "Petrol",
                Transmission = "Manual",
                ImagePath = "fiat.png",
                AvailableFrom = today,
                AvailableTo = today.AddDays(120),
                OwnerId = defaultOwnerId,
                RatePerDay = 8000m,
                CarApprovalStatusId = defaultApprovalStatusId
            },
            new()
            {
                CarId = Guid.NewGuid(),
                Name = "Ford",
                Model = "Mustang",
                Description = "An iconic American muscle coupe.",
                Features = "V8 Engine, Sync 3, Ambient Lighting",
                Year = 2023,
                FuelType = "Petrol",
                Transmission = "Automatic",
                ImagePath = "mustang.png",
                AvailableFrom = today,
                AvailableTo = today.AddDays(120),
                OwnerId = defaultOwnerId,
                RatePerDay = 1200m,
                CarApprovalStatusId = defaultApprovalStatusId
            },
            new()
            {
                CarId = Guid.NewGuid(),
                Name = "Honda",
                Model = "Civic",
                Description = "Reliable compact sedan with modern tech.",
                Features = "Lane Assist, Android Auto, Eco Mode",
                Year = 2022,
                FuelType = "Petrol",
                Transmission = "CVT",
                ImagePath = "civic.png",
                AvailableFrom = today,
                AvailableTo = today.AddDays(120),
                OwnerId = defaultOwnerId,
                RatePerDay = 9000m,
                CarApprovalStatusId = defaultApprovalStatusId
            },
            new()
            {
                CarId = Guid.NewGuid(),
                Name = "Land Rover",
                Model = "Discovery",
                Description = "Luxury SUV with off-road capabilities.",
                Features = "4WD, Panoramic Roof, Terrain Response",
                Year = 2023,
                FuelType = "Diesel",
                Transmission = "Automatic",
                ImagePath = "Discovery.png",
                AvailableFrom = today,
                AvailableTo = today.AddDays(120),
                OwnerId = defaultOwnerId,
                RatePerDay = 15000m,
                CarApprovalStatusId = defaultApprovalStatusId
            },
            new()
            {
                CarId = Guid.NewGuid(),
                Name = "Nissan",
                Model = "GTR R35",
                Description = "Legendary high-performance AWD coupe.",
                Features = "Launch Control, Twin Turbo, Bose Sound",
                Year = 2022,
                FuelType = "Petrol",
                Transmission = "Automatic",
                ImagePath = "nissangtr.png",
                AvailableFrom = today,
                AvailableTo = today.AddDays(120),
                OwnerId = defaultOwnerId,
                RatePerDay = 45000m,
                CarApprovalStatusId = defaultApprovalStatusId
            }
        };

            context.Cars.AddRange(cars);
            await context.SaveChangesAsync();
        }
    }

}
