using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommand : IRequest<Guid>
    {
        public Guid CarId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalCost { get; set; }
        public string PickupLocation { get; set; } = null!;
        public string DropLocation { get; set; } = null!;

        public Guid UserId { get; set; }
    }
}
