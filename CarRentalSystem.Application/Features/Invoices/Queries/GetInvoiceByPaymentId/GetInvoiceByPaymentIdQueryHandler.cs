using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Invoice;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Invoices.Queries.GetInvoiceByPaymentId
{
    public class GetInvoiceByPaymentIdQueryHandler : IRequestHandler<GetInvoiceByPaymentIdQuery, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public GetInvoiceByPaymentIdQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<InvoiceDto> Handle(GetInvoiceByPaymentIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetInvoiceByPaymentIdAsync(request.PaymentId, cancellationToken);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found for this payment.");

            return _mapper.Map<InvoiceDto>(invoice);
        }
    }
}
