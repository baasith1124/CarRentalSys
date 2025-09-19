using CarRentalSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        public string? NICNumber { get; set; }
        public string? Address { get; set; }
        public string? ProfileImagePath { get; set; }

        public ICollection<Car> OwnedCars { get; set; } = new List<Car>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<KYCUpload> KYCUploads { get; set; } = new List<KYCUpload>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
