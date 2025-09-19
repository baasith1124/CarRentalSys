namespace CarRentalSystem.Application.Contracts.AdminAssistant
{
    public class OpenRouterRequest
    {
        public string Query { get; set; } = string.Empty;
        public string Model { get; set; } = "openai/gpt-4o"; // Default to GPT-4o, can be changed to GPT-5 when available
        public double Temperature { get; set; } = 0.1; // Low temperature for consistent responses
        public int MaxTokens { get; set; } = 1000;
    }
}
