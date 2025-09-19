using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Notifications.Commands.MarkNotificationAsSeen
{
    public class MarkNotificationAsSeenCommand : IRequest<bool>
    {
        public Guid NotificationId { get; set; }
    }

}
