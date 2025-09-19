using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetPaymentStatusIdByName
{
    public class GetPaymentStatusIdByNameQuery : IRequest<Guid>
    {
        public string StatusName { get; set; } = null!;

        public GetPaymentStatusIdByNameQuery(string statusName)
        {
            StatusName = statusName;
        }
    }
}
