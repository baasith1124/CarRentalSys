using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Invoices.Commands.CreateInvoice
{
    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Guid>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = new Invoice
            {
                InvoiceId = Guid.NewGuid(),
                PaymentId = request.PaymentId,
                FilePath = request.FilePath,
                GeneratedAt = DateTime.UtcNow
            };

            return await _invoiceRepository.CreateInvoiceAsync(invoice, cancellationToken);
        }
    }
}
