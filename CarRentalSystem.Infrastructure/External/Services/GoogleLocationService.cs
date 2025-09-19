using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.External.Services
{
    public class GoogleLocationService : ILocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GoogleLocationService(HttpClient httpClient, IOptions<GoogleSettings> options)
        {
            _httpClient = httpClient;
            _apiKey = options.Value.PlacesApiKey;
        }

        public async Task<string[]> GetPlaceSuggestionsAsync(string input, CancellationToken cancellationToken)
        {
            var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(input)}&types=(cities)&components=country:lk&key={_apiKey}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var predictions = doc.RootElement.GetProperty("predictions");
            return predictions.EnumerateArray()
                              .Select(p => p.GetProperty("description").GetString()!)
                              .ToArray();
        }

        //private class GooglePlacesResponse
        //{
        //    public List<Prediction> Predictions { get; set; } = new();
        //}

        //private class Prediction
        //{
        //    public string Description { get; set; } = string.Empty;
        //}
    }
}
