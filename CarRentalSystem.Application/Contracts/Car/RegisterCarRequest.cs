using CarRentalSystem.Application.Contracts.CarDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.Car
{
    public class RegisterCarRequest
    {
        public string Name { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? ImagePath { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public Guid OwnerId { get; set; }
        public List<CarDocumentDto> Documents { get; set; } = new(); // For uploading
    }
}
