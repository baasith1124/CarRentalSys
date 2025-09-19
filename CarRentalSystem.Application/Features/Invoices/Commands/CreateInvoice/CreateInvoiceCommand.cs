using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Invoices.Commands.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<Guid>
    {
        public Guid PaymentId { get; set; }
        public string FilePath { get; set; } = null!;
    }
}
