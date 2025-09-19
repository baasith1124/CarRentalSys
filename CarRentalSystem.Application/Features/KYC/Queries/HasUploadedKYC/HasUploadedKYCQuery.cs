using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Queries.HasUploadedKYC
{
    public class HasUploadedKYCQuery : IRequest<bool>
    {
        public Guid UserId { get; set; }

        public HasUploadedKYCQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
