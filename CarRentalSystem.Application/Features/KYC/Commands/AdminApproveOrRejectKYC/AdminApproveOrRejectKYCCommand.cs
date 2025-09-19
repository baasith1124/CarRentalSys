using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Commands.AdminApproveOrRejectKYC
{
    public class AdminApproveOrRejectKYCCommand : IRequest<bool>
    {
        public Guid KYCId { get; set; }
        public string NewStatus { get; set; } = null!; // Should be "Approved" or "Rejected"
        public string? Remarks { get; set; } // Optional admin remarks
        public bool IsApproved { get; set; }
    }
}
