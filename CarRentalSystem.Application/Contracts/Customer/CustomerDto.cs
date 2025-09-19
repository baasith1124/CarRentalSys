using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ProfileImagePath { get; set; }
        public string? NIC { get; set; }
        public string? Address { get; set; }
        public bool IsKycVerified { get; set; }
    }
}
