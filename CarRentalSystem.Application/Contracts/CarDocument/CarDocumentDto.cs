using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.CarDocument
{
    public class CarDocumentDto
    {
        public Guid DocumentId { get; set; }
        public Guid CarId { get; set; }
        public string DocumentType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}
