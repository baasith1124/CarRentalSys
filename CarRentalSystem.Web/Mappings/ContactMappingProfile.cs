using CarRentalSystem.Application.Features.ContactMessages.Commands.CreateContactMessage;
using CarRentalSystem.Web.ViewModels.ContactMessage;
using AutoMapper;

namespace CarRentalSystem.Web.Mappings
{
    public class ContactMappingProfile : Profile
    {
        public ContactMappingProfile()
        {
            CreateMap<ContactMessageViewModel, CreateContactMessageCommand>();
        }
    }
}
