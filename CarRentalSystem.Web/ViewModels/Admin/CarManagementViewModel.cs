using CarRentalSystem.Application.Contracts.Car;

namespace CarRentalSystem.Web.ViewModels.Admin
{
    public class CarManagementViewModel
    {
        public List<CarDto> PendingCars { get; set; } = new();
        public List<CarDto> ApprovedCars { get; set; } = new();
        public List<CarDto> RejectedCars { get; set; } = new();
    }
}
