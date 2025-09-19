using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Repositories
{
    public class EfNotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public EfNotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateNotificationAsync(Notification notification, CancellationToken cancellationToken)
        {
            await _context.Notifications.AddAsync(notification, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return notification.NotificationId;
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> MarkAsSeenAsync(Guid notificationId, CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId, cancellationToken);

            if (notification == null) return false;

            notification.Seen = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId, CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId, cancellationToken);

            if (notification == null) return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
