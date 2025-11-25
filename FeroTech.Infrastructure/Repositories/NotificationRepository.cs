using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Infrastructure.Data;
using FeroTech.Infrastructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeroTech.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _context.Notifications.CountAsync(n => !n.IsRead);
        }

        public async Task AddAsync(string message, string module, string actionType)
        {
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                Message = message,
                Module = module,
                ActionType = actionType,
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync()
        {
            var notifications = await _context.Notifications.Where(n => !n.IsRead).ToListAsync();
            notifications.ForEach(n => n.IsRead = true);
            await _context.SaveChangesAsync();
        }
    }
}