using DataAccess.Models;

namespace DataAccess.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetNotificationsAsync();
    Task<Notification?> GetNotification(long userId);
    Task AddNotificationAsync(Notification notification);
    Task UpdateNotificationAsync(Notification notification);
    Task DeleteNotificationAsync(long userId);
}
