namespace CarRentalSystem.Application.Contracts.AdminAssistant
{
    public class OpenRouterResponse
    {
        public string Content { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}
