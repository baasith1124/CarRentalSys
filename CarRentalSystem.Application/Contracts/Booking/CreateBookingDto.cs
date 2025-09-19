using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.Booking
{
    public class CreateBookingDto
    {
        public Guid CarId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalCost { get; set; }
    }
}
