using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ProfileImagePath { get; set; }
        public string? NIC { get; set; }
        public string? Address { get; set; }
        public bool IsKycVerified { get; set; } = false;

        // Navigation
        public ICollection<Booking>? Bookings { get; set; }
        
        public ICollection<KYCUpload> KYCUploads { get; set; } = new List<KYCUpload>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
