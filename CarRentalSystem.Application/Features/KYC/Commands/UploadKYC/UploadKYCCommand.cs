using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Command.UploadKYC
{
    public class UploadKYCCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string DocumentType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
    }
}
