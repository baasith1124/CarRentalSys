using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarApproval.Commands.ApproveOrRejectCar
{
    public class ApproveOrRejectCarCommand : IRequest<bool>
    {
        public Guid CarId { get; set; }
        public Guid NewStatusId { get; set; }
    }
}
