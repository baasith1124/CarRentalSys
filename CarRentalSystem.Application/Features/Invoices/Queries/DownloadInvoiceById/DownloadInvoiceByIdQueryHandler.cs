using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Invoices.Queries.DownloadInvoiceById
{
    public class DownloadInvoiceByIdQueryHandler : IRequestHandler<DownloadInvoiceByIdQuery, DownloadInvoiceResult>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public DownloadInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<DownloadInvoiceResult> Handle(DownloadInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);
            if (invoice == null || string.IsNullOrWhiteSpace(invoice.FilePath))
                throw new FileNotFoundException("Invoice not found or file path missing.");

            if (!File.Exists(invoice.FilePath))
                throw new FileNotFoundException("Invoice PDF file not found on disk.");

            var fileBytes = await File.ReadAllBytesAsync(invoice.FilePath, cancellationToken);
            var fileName = Path.GetFileName(invoice.FilePath);

            return new DownloadInvoiceResult
            {
                FileContent = fileBytes,
                FileName = fileName
            };
        }
    }
}
