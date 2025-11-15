using FeroTech.Infrastructure.Domain.Entities;

namespace FeroTech.Infrastructure.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetAllAsync();
        Task AddAsync(string message, string module, string actionType);
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync();
        Task<int> GetUnreadCountAsync();
    }
}
