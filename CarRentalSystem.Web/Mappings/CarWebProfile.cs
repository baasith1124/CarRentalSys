using AutoMapper;
using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Web.ViewModels.Car;

namespace CarRentalSystem.Web.Mappings
{
    public class CarWebProfile : Profile
    {
        public CarWebProfile()
        {
            CreateMap<CarDto, CarViewModel>();
        }
    }
}
