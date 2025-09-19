using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;

namespace CarRentalSystem.Application.Features.Cars.Queries.GetCarsByApprovalStatus
{
    public class GetCarsByApprovalStatusQuery : IRequest<List<Car>>
    {
        public string ApprovalStatus { get; set; } = string.Empty;
        
        public GetCarsByApprovalStatusQuery() { }
        
        public GetCarsByApprovalStatusQuery(string approvalStatus)
        {
            ApprovalStatus = approvalStatus;
        }
    }
}
