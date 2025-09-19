using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Web.ViewModels.Payment
{
    public class PaymentCheckoutViewModel
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string CarName { get; set; } = null!;
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string CustomerName { get; set; } = null!;
        public int Days => Math.Max(1, (ReturnDate - PickupDate).Days);
        public decimal DailyRate => Days > 0 ? Amount / Days : Amount;
        
        // Additional properties for better payment experience
        public string PickupLocation { get; set; } = string.Empty;
        public string DropLocation { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public string CarImagePath { get; set; } = string.Empty;
    }
}
