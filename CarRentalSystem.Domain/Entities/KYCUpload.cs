using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Domain.Entities
{
    public class KYCUpload
    {
        [Key]
        public Guid KYCId { get; set; }
        public Guid UserId { get; set; }
        //public ApplicationUser User { get; set; }

        public string DocumentType { get; set; } = null!; // NIC, Passport, Selfie
        public string FilePath { get; set; } = null!;
        public string Status { get; set; } = "Pending";
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

}
