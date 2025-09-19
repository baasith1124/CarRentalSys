using CarRentalSystem.Application.Contracts.KYC;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Queries.GetPendingKYCs
{
    public class GetPendingKYCsQuery : IRequest<List<KYCDto>>
    {
    }
}
