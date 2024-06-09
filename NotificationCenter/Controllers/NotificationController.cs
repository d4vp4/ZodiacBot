using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NotificationCenter.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class NotificationController(INotificationRepository notificationRepository) : Controller
{
    [HttpGet("{userId:long}")]
    public async Task<Notification?> GetNotification(long userId)
    {
        var notification = await notificationRepository.GetNotification(userId);

        return notification;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] Notification notification)
    {
        if (await GetNotification(notification.UserId) is not null)
            return BadRequest("Ви вже підписані на отримання сповіщень");

        await notificationRepository.AddNotificationAsync(notification);

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateNotification([FromBody] Notification notification)
    {
        if (await GetNotification(notification.UserId) is null)
            return NotFound("Ви не підписані на отримання сповіщень");

        await notificationRepository.UpdateNotificationAsync(notification);

        return Ok();
    }

    [HttpDelete("{userId:long}")]
    public async Task<IActionResult> DeleteNotification(long userId)
    {
        if (await GetNotification(userId) is null)
            return NotFound("Ви не підписані на отримання сповіщень");

        await notificationRepository.DeleteNotificationAsync(userId);

        return Ok();
    }
}
