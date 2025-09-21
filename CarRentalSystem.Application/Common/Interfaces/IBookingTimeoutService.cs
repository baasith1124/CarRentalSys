namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IBookingTimeoutService
    {
        Task ProcessBookingTimeoutsAsync(CancellationToken cancellationToken);
    }
}
