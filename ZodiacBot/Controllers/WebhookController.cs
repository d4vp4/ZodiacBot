using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace ZodiacBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController(IZodiacService zodiacService) : Controller
{
    [HttpPost("telegram")]
    public async Task<IActionResult> TelegramWebhook([FromBody] Update update)
    {
        return await zodiacService.ProcessRequestAsync(update);
    }

    [HttpPost("notification")]
    public async Task<IActionResult> NotificationWebhook([FromBody] List<NotificationDto> notifications)
    {
        return await zodiacService.ProcessNotificationsAsync(notifications);
    }
}
