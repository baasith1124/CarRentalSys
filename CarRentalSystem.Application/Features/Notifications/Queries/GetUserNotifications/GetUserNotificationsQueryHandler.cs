using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Notification;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Notifications.Queries.GetUserNotifications
{
    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, List<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public GetUserNotificationsQueryHandler(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<List<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(request.UserId, cancellationToken);

            return _mapper.Map<List<NotificationDto>>(notifications);
        }
    }

}
