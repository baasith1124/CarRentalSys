using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Web.Models;
using CarRentalSystem.Web.ViewModels.Car;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace CarRentalSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger,
            ICarRepository carRepository,
            IMapper mapper)
        {
            _logger = logger;
            _carRepository = carRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carRepository.GetTopAvailableCarsAsync();
            var carViewModels = _mapper.Map<List<CarViewModel>>(cars);
            return View(carViewModels);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
