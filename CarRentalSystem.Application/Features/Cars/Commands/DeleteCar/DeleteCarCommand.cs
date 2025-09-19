using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.DeleteCar
{
    public class DeleteCarCommand : IRequest<bool>
    {
        public Guid CarId { get; set; }
    }
}
