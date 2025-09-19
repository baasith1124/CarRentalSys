using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.Notification
{
    public class NotificationDto
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string Message { get; set; } = null!;
        public bool Seen { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
