using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;

namespace CarRentalSystem.Application.Features.Cars.Queries.GetCarsByApprovalStatus
{
    public class GetCarsByApprovalStatusQueryHandler : IRequestHandler<GetCarsByApprovalStatusQuery, List<Car>>
    {
        private readonly ICarRepository _carRepository;

        public GetCarsByApprovalStatusQueryHandler(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<List<Car>> Handle(GetCarsByApprovalStatusQuery request, CancellationToken cancellationToken)
        {
            return await _carRepository.GetCarsByApprovalStatusAsync(request.ApprovalStatus, cancellationToken);
        }
    }
}
