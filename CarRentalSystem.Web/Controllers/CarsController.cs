﻿using CarRentalSystem.Application.Features.Cars.Queries.SearchCars;
using CarRentalSystem.Application.Features.Cars.Queries.GetCarById;
using CarRentalSystem.Application.Common.Settings;
using CarRentalSystem.Web.ViewModels.Car;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CarRentalSystem.Web.Controllers
{
    public class CarsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly GoogleSettings _googleSettings;

        public CarsController(IMediator mediator, IOptions<GoogleSettings> googleSettings)
        {
            _mediator = mediator;
            _googleSettings = googleSettings.Value;
        }

        [HttpPost]
        [Route("cars/search")]
        public async Task<IActionResult> Search([FromForm] SearchCarsQuery query)
        {
            var cars = await _mediator.Send(query);

            // Store search parameters in session
            HttpContext.Session.SetString("PickupDate", query.PickupDate.ToString("o")); // ISO format
            HttpContext.Session.SetString("DropDate", query.DropDate.ToString("o"));
            HttpContext.Session.SetString("PickupLocation", query.PickupLocation);
            HttpContext.Session.SetString("DropLocation", query.DropLocation);

            // Extract unique values from the result for filters
            var brands = cars.Select(c => c.Name).Distinct().ToList();
            var transmissions = cars.Select(c => c.Transmission).Distinct().ToList();
            var fuelTypes = cars.Select(c => c.FuelType).Distinct().ToList();

            var resultViewModel = new CarSearchResultViewModel
            {
                Cars = cars,
                Brand = query.Brand,
                Transmission = query.Transmission,
                FuelType = query.FuelType,
                PickupDate = query.PickupDate,
                DropDate = query.DropDate,
                Brands = brands,
                Transmissions = transmissions,
                FuelTypes = fuelTypes,
                MinYear = query.MinYear ?? 2000,
                MaxYear = query.MaxYear ?? DateTime.Now.Year
            };

            // Pass Google Places API key to the view
            ViewBag.GooglePlacesApiKey = _googleSettings.PlacesApiKey;

            return View("SearchResults", resultViewModel);
        }

        [HttpGet]
        [Route("cars/details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var query = new GetCarByIdQuery(id);
            var car = await _mediator.Send(query);
            
            if (car == null)
            {
                return NotFound();
            }

            var carViewModel = new CarDetailsViewModel
            {
                CarId = car.CarId,
                Name = car.Name,
                Model = car.Model,
                Year = car.Year,
                Color = "Not specified", // CarDto doesn't have Color property
                Transmission = car.Transmission ?? "Not specified",
                FuelType = car.FuelType ?? "Not specified",
                SeatingCapacity = 5, // Default value since CarDto doesn't have this property
                RatePerDay = car.RatePerDay,
                AvailableFrom = car.AvailableFrom,
                AvailableTo = car.AvailableTo,
                Description = car.Description ?? "No description available",
                ImagePath = car.ImagePath ?? "",
                OwnerName = car.OwnerName,
                OwnerEmail = car.OwnerEmail,
                OwnerPhone = "Not available", // CarDto doesn't have OwnerPhone property
                Location = "Not specified", // CarDto doesn't have Location property
                Features = car.Features?.Split(',').ToList() ?? new List<string>()
            };

            return View(carViewModel);
        }

    }
}
