using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;

namespace CarRentalSystem.Application.Features.KYC.Queries.GetAllKYCUploads
{
    public class GetAllKYCUploadsQuery : IRequest<List<KYCUpload>>
    {
    }
}
