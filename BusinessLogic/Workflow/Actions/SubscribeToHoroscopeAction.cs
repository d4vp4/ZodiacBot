using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using BusinessLogic.Utilities;
using DataAccess.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DataAccess.Models.User;

namespace BusinessLogic.Workflow.Actions;

public class SubscribeToHoroscopeAction(ITelegramBotClient client, INotificationService service) : IWorkflowAction
{
    public async Task ProcessAsync(User user, Update update)
    {
        if (update.Message!.Text == BotCommands.Unsubscribe)
        {
            var deleteResponse = await service.DeleteNotificationAsync(user.UserId);

            if (deleteResponse is not null)
            {
                await client.SendTextMessageAsync(user.UserId, deleteResponse);
                return;
            }

            user.NextAction = null;

            await client.SendTextMessageAsync(user.UserId, "Ви успішно відписались від гороскопу");

            return;
        }

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

        Period period;

        switch (text[1].ToLower())
        {
            case "день":
                period = Period.Daily;
                break;
            case "тиждень":
                period = Period.Weekly;
                break;
            case "місяць":
                period = Period.Monthly;
                break;
            default:
                await client.SendTextMessageAsync(user.UserId, $"Невірний період: {text[1]}");
                return;
        }

        var response = await service.CreateNotificationAsync(new NotificationDto(user.UserId, period, zodiac));

        if (response is not null)
        {
            await client.SendTextMessageAsync(user.UserId, response);
            return;
        }

        user.NextAction = null;

        await client.SendTextMessageAsync(user.UserId, $"Ви успішно підписались на гороскоп по знаку зодіаку *{zodiac.TranslateEnglishToUkrainian()}* на період *{text[1].ToLower()}*", parseMode: ParseMode.MarkdownV2);
    }
}
