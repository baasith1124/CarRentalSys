using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.Invoice
{
    public class InvoiceDto
    {
        public Guid InvoiceId { get; set; }
        public Guid BookingId { get; set; }
        public string FilePath { get; set; } = null!;
        public DateTime GeneratedAt { get; set; }
    }
}
