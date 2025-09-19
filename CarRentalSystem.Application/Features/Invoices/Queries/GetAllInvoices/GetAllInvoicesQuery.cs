using CarRentalSystem.Application.Contracts.Invoice;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Invoices.Queries.GetAllInvoices
{
    public class GetAllInvoicesQuery : IRequest<List<InvoiceDto>>
    {
    }
}
