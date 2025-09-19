using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Domain.Entities
{
    public class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        //public ApplicationUser User { get; set; }

        public string? Title { get; set; }
        public string Message { get; set; } = null!;
        public bool Seen { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
