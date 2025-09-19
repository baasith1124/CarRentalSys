using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.AdminAssistant;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace CarRentalSystem.Application.Features.AdminAssistant
{
    public class OpenRouterService : IOpenRouterService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://openrouter.ai/api/v1";

        public OpenRouterService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenRouter:ApiKey"] ?? throw new ArgumentNullException("OpenRouter:ApiKey not configured");
            
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://your-app.com"); // Replace with your app URL
            _httpClient.DefaultRequestHeaders.Add("X-Title", "Car Rental Admin Assistant");
        }

        public async Task<OpenRouterResponse> ProcessQueryAsync(OpenRouterRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var openRouterRequest = new
                {
                    model = request.Model,
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = request.Query
                        }
                    },
                    temperature = request.Temperature,
                    max_tokens = request.MaxTokens
                };

                var json = JsonSerializer.Serialize(openRouterRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content, cancellationToken);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    return new OpenRouterResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"OpenRouter API error: {response.StatusCode} - {errorContent}"
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                
                // Log the response for debugging
                Console.WriteLine($"OpenRouter Response: {responseContent}");
                
                var openRouterResponse = JsonSerializer.Deserialize<OpenRouterApiResponse>(responseContent);

                if (openRouterResponse?.Choices?.FirstOrDefault()?.Message?.Content == null)
                {
                    return new OpenRouterResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid response from OpenRouter API"
                    };
                }

                return new OpenRouterResponse
                {
                    Content = openRouterResponse.Choices.First().Message.Content,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OpenRouter Exception: {ex.Message}");
                return new OpenRouterResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }

    public class OpenRouterApiResponse
    {
        public List<Choice>? Choices { get; set; }
    }

    public class Choice
    {
        public Message? Message { get; set; }
    }

    public class Message
    {
        public string? Content { get; set; }
    }
}
