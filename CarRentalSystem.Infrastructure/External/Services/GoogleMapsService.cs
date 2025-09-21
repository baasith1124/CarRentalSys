using CarRentalSystem.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.External.Services
{
    public class GoogleMapsService : IGoogleMapsService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public GoogleMapsService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> GetApiKeyAsync()
        {
            return await Task.FromResult(_configuration["GoogleMaps:ApiKey"] ?? string.Empty);
        }

        public async Task<object> GetDirectionsAsync(double pickupLat, double pickupLng, double dropLat, double dropLng)
        {
            var apiKey = await GetApiKeyAsync();
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Google Maps API key is not configured");
            }

            var origin = $"{pickupLat},{pickupLng}";
            var destination = $"{dropLat},{dropLng}";
            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin}&destination={destination}&key={apiKey}";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                return JsonSerializer.Deserialize<object>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get directions from Google Maps API: {ex.Message}", ex);
            }
        }
    }
}
