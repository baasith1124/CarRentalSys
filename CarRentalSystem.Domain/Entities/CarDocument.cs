using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Domain.Entities
{
    public class CarDocument
    {
        [Key]
        public Guid DocumentId { get; set; }
        public Guid CarId { get; set; }
        public Car Car { get; set; } = null!;

        public string DocumentType { get; set; } = null!;  // From enum: RCBook, Insurance, etc.
        public string FilePath { get; set; } = null!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

}
