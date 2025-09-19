using AutoMapper;
using CarRentalSystem.Application.Contracts.CarApprovalStatus;
using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Mappings
{
    public class CarApprovalStatusProfile : Profile
    {
        public CarApprovalStatusProfile()
        {
            CreateMap<CarApprovalStatus, CarApprovalStatusDto>();
        }
    }
}
