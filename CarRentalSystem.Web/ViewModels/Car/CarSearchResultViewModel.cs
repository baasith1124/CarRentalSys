using CarRentalSystem.Application.Contracts.Car;

namespace CarRentalSystem.Web.ViewModels.Car
{
    public class CarSearchResultViewModel
    {
        public CarSearchResultViewModel()
        {
            Cars = new List<CarDto>();
            Brands = new List<string>();
            Transmissions = new List<string>();
            FuelTypes = new List<string>();
        }

        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DropDate { get; set; }
        public string? Brand { get; set; }
        public string? Transmission { get; set; }
        public string? FuelType { get; set; }
        public List<CarDto> Cars { get; set; }
        public List<string> Brands { get; set; }
        public List<string> Transmissions { get; set; }
        public List<string> FuelTypes { get; set; }
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
    }
}
