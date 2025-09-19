using CarRentalSystem.Application.Contracts.Notification;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Notifications.Queries.GetUserNotifications
{
    public class GetUserNotificationsQuery : IRequest<List<NotificationDto>>
    {
        public Guid UserId { get; set; }
    }

}
