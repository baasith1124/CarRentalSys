using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Notifications.Commands.MarkNotificationAsSeen
{
    public class MarkNotificationAsSeenCommandHandler : IRequestHandler<MarkNotificationAsSeenCommand, bool>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkNotificationAsSeenCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<bool> Handle(MarkNotificationAsSeenCommand request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.MarkAsSeenAsync(request.NotificationId, cancellationToken);
        }
    }

}
