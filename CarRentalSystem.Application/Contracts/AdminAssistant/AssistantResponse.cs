namespace CarRentalSystem.Application.Contracts.AdminAssistant
{
    public class AssistantResponse
    {
        public string Message { get; set; } = string.Empty;
        public string ResponseType { get; set; } = string.Empty; // "text", "download", "chart"
        public string? DownloadUrl { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}
