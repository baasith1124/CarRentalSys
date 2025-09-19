using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Notifications.Commands.SendNotification
{
    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Guid>
    {
        private readonly INotificationRepository _notificationRepository;

        public SendNotificationCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Guid> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Message = request.Message,
                Title = request.Title,
                CreatedAt = DateTime.UtcNow,
                Seen = false
            };

            return await _notificationRepository.CreateNotificationAsync(notification, cancellationToken);
        }
    }
}
