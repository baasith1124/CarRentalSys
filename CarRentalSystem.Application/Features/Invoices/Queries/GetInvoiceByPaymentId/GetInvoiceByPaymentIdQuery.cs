using CarRentalSystem.Application.Contracts.Invoice;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Invoices.Queries.GetInvoiceByPaymentId
{
    public class GetInvoiceByPaymentIdQuery : IRequest<InvoiceDto>
    {
        public Guid PaymentId { get; set; }

        public GetInvoiceByPaymentIdQuery(Guid paymentId)
        {
            PaymentId = paymentId;
        }
    }
}
