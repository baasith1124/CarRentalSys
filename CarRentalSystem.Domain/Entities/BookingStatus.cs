using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Domain.Entities
{
    public class BookingStatus
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;  // e.g., Pending, Approved, Cancelled
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
