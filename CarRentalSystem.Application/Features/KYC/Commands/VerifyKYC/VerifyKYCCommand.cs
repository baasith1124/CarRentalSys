using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Commands.VerifyKYC
{
    public class VerifyKYCCommand : IRequest<bool>
    {
        public Guid KYCId { get; set; }
        public string NewStatus { get; set; } = null!; // Approved / Rejected
    }
}
