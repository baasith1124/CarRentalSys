using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Invoices.Queries.DownloadInvoiceById
{
    public class DownloadInvoiceByIdQuery : IRequest<DownloadInvoiceResult>
    {
        public Guid InvoiceId { get; set; }
    }

    public class DownloadInvoiceResult
    {
        public byte[] FileContent { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = "application/pdf";
    }
}
