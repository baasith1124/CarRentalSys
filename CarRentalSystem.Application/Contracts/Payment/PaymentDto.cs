using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.Payment
{
    public class PaymentDto
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string? StripeTxnId { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
