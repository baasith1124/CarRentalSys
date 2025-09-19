using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Application.Contracts.CarDocument;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.CarDocuments.Queries.GetDocumentsByCarId
{
    public class GetDocumentsByCarIdQuery : IRequest<List<CarDocumentDto>>
    {
        public Guid CarId { get; set; }
    }
}
