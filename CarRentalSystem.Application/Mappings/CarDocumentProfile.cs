using AutoMapper;
using CarRentalSystem.Application.Contracts.CarDocument;
using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Mappings
{
    public class CarDocumentProfile : Profile
    {
        public CarDocumentProfile()
        {
            CreateMap<CarDocument, CarDocumentDto>().ReverseMap();
        }
    }
}
