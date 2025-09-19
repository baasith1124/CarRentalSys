using AutoMapper;
using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Mappings
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            CreateMap<Car, CarDto>()
                .ForMember(dest => dest.ApprovalStatus, opt => opt.MapFrom(src => src.CarApprovalStatus != null ? src.CarApprovalStatus.Name : "Unknown"))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => "Unknown")) // Will be populated by repository
                .ForMember(dest => dest.OwnerEmail, opt => opt.MapFrom(src => "Unknown")) // Will be populated by repository
                .ForMember(dest => dest.CarApprovalStatus, opt => opt.MapFrom(src => src.CarApprovalStatus != null ? src.CarApprovalStatus.Name : "Unknown"));
        }
    }
}
