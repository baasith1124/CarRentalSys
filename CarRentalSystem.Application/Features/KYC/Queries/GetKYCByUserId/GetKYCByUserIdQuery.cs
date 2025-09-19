using CarRentalSystem.Application.Contracts.KYC;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Queries.GetKYCByUserId
{
    public class GetKYCByUserIdQuery : IRequest<List<KYCDto>>
    {
        public Guid UserId { get; set; }

        public GetKYCByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
