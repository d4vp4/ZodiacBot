using BusinessLogic.Interfaces;
using BusinessLogic.Utilities;
using DataAccess.Enums;
using DataAccess.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DataAccess.Models.User;

namespace BusinessLogic.Workflow.Actions;

public class GetHoroscopeAction(ITelegramBotClient client, IZodiacService service) : IWorkflowAction
{
    public async Task ProcessAsync(User user, Update update)
    {
        var text = update.Message!.Text!.Split(' ');

        if (text.Length != 2)
        {
            await client.SendTextMessageAsync(user.UserId, "Невірний формат, будь ласка, введіть знак зодіаку та період через пробіл");
            return;
        }

        var zodiac = text[0].TranslateUkrainianToEnglish();

        if (zodiac is null)
        {
            await client.SendTextMessageAsync(user.UserId, $"Невірний знак зодіаку: {text[0]}");
            return;
        }

        var period = text[1].ToLower();
        string? message;

        switch (period)
        {
            case "день":
                message = await service.GetHoroscopeAsync(zodiac, Period.Daily);
                break;
            case "тиждень":
                message = await service.GetHoroscopeAsync(zodiac, Period.Weekly);
                break;
            case "місяць":
                message = await service.GetHoroscopeAsync(zodiac, Period.Monthly);
                break;
            default:
                await client.SendTextMessageAsync(user.UserId, $"Невірний період: {text[1]}");
                return;
        }

        if (message is null)
        {
            await client.SendTextMessageAsync(user.UserId, "Не вдалося отримати гороскоп");
            return;
        }

        await client.SendTextMessageAsync(user.UserId, $"Гороскоп на {period} для {zodiac.TranslateEnglishToUkrainian()}:\n\n{message}");

        user.NextAction = null;
    }
}
