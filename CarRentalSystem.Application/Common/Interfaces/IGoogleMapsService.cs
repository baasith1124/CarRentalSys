using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IGoogleMapsService
    {
        Task<string> GetApiKeyAsync();
        Task<object> GetDirectionsAsync(double pickupLat, double pickupLng, double dropLat, double dropLng);
    }
}
