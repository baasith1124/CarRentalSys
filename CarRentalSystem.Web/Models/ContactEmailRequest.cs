namespace CarRentalSystem.Web.Models
{
    public class ContactEmailRequest
    {
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string MessageSubject { get; set; } = string.Empty;
        public string MessageContent { get; set; } = string.Empty;
    }
}
