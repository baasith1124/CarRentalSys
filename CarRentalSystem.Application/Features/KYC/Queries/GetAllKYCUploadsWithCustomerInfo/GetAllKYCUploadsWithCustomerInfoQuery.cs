using CarRentalSystem.Application.Contracts.KYC;
using MediatR;

namespace CarRentalSystem.Application.Features.KYC.Queries.GetAllKYCUploadsWithCustomerInfo
{
    public class GetAllKYCUploadsWithCustomerInfoQuery : IRequest<List<KYCUploadDto>>
    {
    }
}
