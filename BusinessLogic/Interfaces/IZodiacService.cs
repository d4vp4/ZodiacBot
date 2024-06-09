using BusinessLogic.DTOs;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace BusinessLogic.Interfaces;

public interface IZodiacService
{
    Task<IActionResult> ProcessRequestAsync(Update update);
    Task<IActionResult> ProcessNotificationsAsync(List<NotificationDto> notifications);
    Task<string?> GetHoroscopeAsync(string zodiac, Period period);
}
