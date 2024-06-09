using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Operations;

public class NotificationRepository(ZodiacDbContext dbContext) : INotificationRepository
{
    public async Task<List<Notification>> GetNotificationsAsync()
    {
        return await dbContext.Notifications.ToListAsync();
    }

    public async Task<Notification?> GetNotification(long userId)
    {
        return await dbContext.Notifications.FirstOrDefaultAsync(n => n.UserId == userId);
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        await dbContext.Notifications.AddAsync(notification);

        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateNotificationAsync(Notification notification)
    {
        dbContext.Notifications.Update(notification);

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteNotificationAsync(long userId)
    {
        var notification = await dbContext.Notifications.FirstOrDefaultAsync(n => n.UserId == userId);

        if (notification is not null)
        {
            dbContext.Notifications.Remove(notification);

            await dbContext.SaveChangesAsync();
        }
    }
}
