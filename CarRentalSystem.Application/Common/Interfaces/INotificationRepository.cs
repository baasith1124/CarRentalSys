using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface INotificationRepository
    {
        Task<Guid> CreateNotificationAsync(Notification notification, CancellationToken cancellationToken);
        Task<List<Notification>> GetNotificationsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> MarkAsSeenAsync(Guid notificationId, CancellationToken cancellationToken);
        Task<bool> DeleteNotificationAsync(Guid notificationId, CancellationToken cancellationToken);
    }
}
