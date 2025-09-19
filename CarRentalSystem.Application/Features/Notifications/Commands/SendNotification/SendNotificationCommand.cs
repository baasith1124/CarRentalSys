using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Notifications.Commands.SendNotification
{
    public class SendNotificationCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string Message { get; set; } = null!;
        public string? Title { get; set; }
    }
}
