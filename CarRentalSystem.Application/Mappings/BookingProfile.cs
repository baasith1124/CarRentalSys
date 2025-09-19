using AutoMapper;
using CarRentalSystem.Application.Contracts.Booking;
using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Application.Mappings
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.CarName, opt => opt.MapFrom(src => src.Car.Name))
                .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src => src.BookingStatus.Name))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.Name))
                .ForMember(dest => dest.PickupLocation, opt => opt.MapFrom(src => "Unknown")) // Will be populated by repository
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
                .ForMember(dest => dest.Car, opt => opt.MapFrom(src => src.Car));

            // Add mapping for Car to CarInfo
            CreateMap<Car, CarInfo>()
                .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model));
        }
    }
}