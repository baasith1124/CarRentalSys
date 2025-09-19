using CarRentalSystem.Application.Features.Bookings.Queries.GetBookingKPIData;
using CarRentalSystem.Application.Features.Customers.Queries.GetCustomerKPIData;
using CarRentalSystem.Application.Features.Cars.Queries.GetCarKPIData;

namespace CarRentalSystem.Web.ViewModels.Admin
{
    public class KPIViewModel
    {
        public BookingKPIDataDto BookingKPIData { get; set; } = new();
        public CustomerKPIDataDto CustomerKPIData { get; set; } = new();
        public CarKPIDataDto CarKPIData { get; set; } = new();
    }
}
