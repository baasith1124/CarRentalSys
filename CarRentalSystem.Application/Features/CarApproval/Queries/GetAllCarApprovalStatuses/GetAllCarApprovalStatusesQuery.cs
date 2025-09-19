using CarRentalSystem.Application.Contracts.CarApprovalStatus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarApproval.Queries.GetAllCarApprovalStatuses
{
    public class GetAllCarApprovalStatusesQuery : IRequest<List<CarApprovalStatusDto>>
    {
    }
}
