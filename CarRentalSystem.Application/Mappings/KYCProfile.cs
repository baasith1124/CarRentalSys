using AutoMapper;
using CarRentalSystem.Application.Contracts.KYC;
using CarRentalSystem.Domain.Entities;

namespace CarRentalSystem.Application.Mappings
{
    public class KYCProfile : Profile
    {
        public KYCProfile()
        {
            CreateMap<KYCUpload, KYCUploadDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => "Unknown")) // Will be populated by repository
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => "Unknown")); // Will be populated by repository
        }
    }
}
