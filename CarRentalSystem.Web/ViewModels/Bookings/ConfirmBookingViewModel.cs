using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Web.ViewModels.Bookings
{
    public class ConfirmBookingViewModel
    {
        [Required]
        public Guid CarId { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        [Required]
        public DateTime DropDate { get; set; }

        [Required]
        [StringLength(100)]
        public string PickupLocation { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string DropLocation { get; set; } = null!;
        
        // Location coordinates
        public double? PickupLatitude { get; set; }
        public double? PickupLongitude { get; set; }
        public double? DropLatitude { get; set; }
        public double? DropLongitude { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Estimated cost must be greater than zero.")]
        public decimal EstimatedCost { get; set; }
    }
}
