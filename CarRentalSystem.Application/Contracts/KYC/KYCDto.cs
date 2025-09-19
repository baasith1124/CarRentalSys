using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.KYC
{
    public class KYCDto
    {
        public Guid KYCId { get; set; }
        public Guid UserId { get; set; }
        public string DocumentType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}
