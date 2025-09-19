using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Domain.Entities
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public Guid PaymentId { get; set; }
        public Payment Payment { get; set; } = null!;

        public string FilePath { get; set; } = null!;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

}
