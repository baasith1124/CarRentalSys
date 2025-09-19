using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Queries.GetAllCars
{
    public class GetAllCarsQueryValidator : AbstractValidator<GetAllCarsQuery>
    {
        public GetAllCarsQueryValidator()
        {
            // No filters to validate now, but ready for future enhancement
        }
    }
}
