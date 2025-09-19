using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Invoice;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Invoices.Queries.GetAllInvoices
{
    public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, List<InvoiceDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public GetAllInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<List<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync(cancellationToken);
            return _mapper.Map<List<InvoiceDto>>(invoices);
        }
    }
}
