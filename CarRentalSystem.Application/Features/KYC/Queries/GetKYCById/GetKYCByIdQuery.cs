using CarRentalSystem.Application.Contracts.KYC;
using MediatR;

namespace CarRentalSystem.Application.Features.KYC.Queries.GetKYCById
{
    public class GetKYCByIdQuery : IRequest<KYCUploadDto?>
    {
        public Guid KYCId { get; set; }

        public GetKYCByIdQuery(Guid kycId)
        {
            KYCId = kycId;
        }
    }
}
