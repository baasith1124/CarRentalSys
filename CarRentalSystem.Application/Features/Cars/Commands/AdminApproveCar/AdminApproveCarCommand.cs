using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Cars.Commands.AdminApproveCar
{
    public class AdminApproveCarCommand : IRequest<bool>
    {
        public Guid CarId { get; set; }
        public string Status { get; set; } = null!; // "Approved" or "Rejected"
        public bool IsApproved { get; set; }
    }
}
