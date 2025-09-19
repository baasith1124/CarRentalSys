using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarApproval.Commands.ApproveOrRejectCar
{
    public class ApproveOrRejectCarCommandHandler : IRequestHandler<ApproveOrRejectCarCommand, bool>
    {
        private readonly ICarApprovalStatusRepository _carApprovalStatusRepository;

        public ApproveOrRejectCarCommandHandler(ICarApprovalStatusRepository carApprovalStatusRepository)
        {
            _carApprovalStatusRepository = carApprovalStatusRepository;
        }

        public async Task<bool> Handle(ApproveOrRejectCarCommand request, CancellationToken cancellationToken)
        {
            return await _carApprovalStatusRepository.UpdateStatusAsync(request.CarId, request.NewStatusId, cancellationToken);
        }
    }
}
