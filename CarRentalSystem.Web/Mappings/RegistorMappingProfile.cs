using AutoMapper;
using CarRentalSystem.Application.Features.Account.Commands.RegisterCustomer;
using CarRentalSystem.Application.Features.Account.Commands.RegisterCustomerWithOTP;
using CarRentalSystem.Web.ViewModels.Account;

namespace CarRentalSystem.Web.Mappings
{
    public class RegistorMappingProfile :Profile
    {
        public RegistorMappingProfile() 
        {
            CreateMap<RegisterCustomerViewModel, RegisterCustomerCommand>();
            CreateMap<RegisterCustomerWithOTPViewModel, RegisterCustomerWithOTPCommand>();
        }
    }
}
