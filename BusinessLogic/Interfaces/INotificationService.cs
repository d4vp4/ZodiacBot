using BusinessLogic.DTOs;

namespace BusinessLogic.Interfaces;

public interface INotificationService
{
    Task<NotificationDto?> GetNotificationByUserAsync(long userId);
    Task<string?> CreateNotificationAsync(NotificationDto notificationDto);
    Task<string?> UpdateNotificationAsync(NotificationDto notificationDto);
    Task<string?> DeleteNotificationAsync(long userId);
}
