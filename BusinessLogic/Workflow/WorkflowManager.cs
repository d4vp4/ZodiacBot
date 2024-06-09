using BusinessLogic.Interfaces;
using BusinessLogic.Utilities;
using BusinessLogic.Workflow.Actions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = DataAccess.Models.User;

namespace BusinessLogic.Workflow;

public class WorkflowManager(ITelegramBotClient client, INotificationService service)
{
    public async Task<bool> ExecuteWorkflowAsync(User user, Update update)
    {
        var command = update.Message!.Text;

        switch (command)
        {
            case $"/{BotCommands.Start}":
                await client.SendTextMessageAsync(user.UserId, "Привіт! Я бот, який допоможе тобі дізнатися свій знак зодіаку.");
                break;
            case $"/{BotCommands.Help}":
                await client.SendTextMessageAsync(user.UserId, $"Введи /menu, щоб побачити меню команд.\n\nЗнаки зодіаку які я розпізнаю:\n{ZodiacTranslator.UkrainianSigns}");
                break;
            case $"/{BotCommands.Menu}":
                await SendMenuAsync(user);
                break;
            case BotCommands.ZodiacCompatibility:
                user.NextAction = nameof(CheckCompatibilityAction);
                await client.SendTextMessageAsync(user.UserId, "Введіть два знаки зодіаку через пробіл, щоб дізнатися їх сумісність\n\n*Приклад: Овен Телець*", parseMode: ParseMode.MarkdownV2, replyMarkup: GetCancelKeyboard("овен телець"));
                break;
            case BotCommands.ZodiacSign:
                user.NextAction = nameof(CheckZodiacByDobAction);
                await client.SendTextMessageAsync(user.UserId, "Введіть свою дату народження у форматі ДД\\-ММ, щоб дізнатися свій знак зодіаку\n\n*Приклад: 20\\-12*", parseMode: ParseMode.MarkdownV2, replyMarkup: GetCancelKeyboard("20-12"));
                break;
            case BotCommands.MoonPhase:
                var phase = new GetMoonPhaseAction(client);
                await phase.ProcessAsync(user, update);
                break;
            case BotCommands.ZodiacCharacteristics:
                user.NextAction = nameof(GetCharacteristicAction);
                await client.SendTextMessageAsync(user.UserId, "Введіть знак зодіаку, щоб дізнатися його характеристику\n\n*Приклад: Овен*", parseMode: ParseMode.MarkdownV2, replyMarkup: GetCancelKeyboard("овен"));
                break;
            case BotCommands.GetHoroscope:
                user.NextAction = nameof(GetHoroscopeAction);
                await client.SendTextMessageAsync(user.UserId, "Введіть знак зодіаку та період \\(день, тиждень, місяць\\) через пробіл, щоб ортимати гороскоп\n\n*Приклад: Овен день*", parseMode: ParseMode.MarkdownV2, replyMarkup: GetCancelKeyboard("овен день"));
                break;
            case BotCommands.SubscribeToHoroscope:
                user.NextAction = nameof(SubscribeToHoroscopeAction);
                await GetUserNotificationAsync(user);
                break;
            case BotCommands.Cancel:
                user.NextAction = null;
                await client.SendTextMessageAsync(user.UserId, "Відміна оперції...");
                await SendMenuAsync(user);
                break;
            default:
                return false;
        }

        return true;
    }

    private async Task SendMenuAsync(User user)
    {
        var keyboard = new ReplyKeyboardMarkup(
        [
            [
                new KeyboardButton(BotCommands.ZodiacCompatibility),
                new KeyboardButton(BotCommands.ZodiacSign)
            ],
            [
                new KeyboardButton(BotCommands.MoonPhase),
                new KeyboardButton(BotCommands.ZodiacCharacteristics)
            ],
            [
                new KeyboardButton(BotCommands.GetHoroscope),
                new KeyboardButton(BotCommands.SubscribeToHoroscope)
            ]
        ]) { ResizeKeyboard = true, OneTimeKeyboard = true, InputFieldPlaceholder = "Обери опцію" };

        user.NextAction = null;

        await client.SendTextMessageAsync(user.UserId, "Обери опцію:", replyMarkup: keyboard, parseMode: ParseMode.MarkdownV2);
    }

    private static ReplyKeyboardMarkup GetCancelKeyboard(string text, string? additionalButton = null)
    {
        List<KeyboardButton> buttons = [new KeyboardButton(BotCommands.Cancel)];

        if (additionalButton is not null)
            buttons.Add(new KeyboardButton(additionalButton));

        var keyboard = new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true, OneTimeKeyboard = true, InputFieldPlaceholder = text };

        return keyboard;
    }

    private async Task GetUserNotificationAsync(User user)
    {
        var notification = await service.GetNotificationByUserAsync(user.UserId);

        if (notification is null)
            await client.SendTextMessageAsync(user.UserId, "Введіть знак зодіаку та період \\(день, тиждень, місяць\\) через пробіл, щоб підписатись на розсилку гороскопа\n\n*Приклад: Овен день*", parseMode: ParseMode.MarkdownV2, replyMarkup: GetCancelKeyboard("овен день"));
        else
            await client.SendTextMessageAsync(user.UserId, $"Введіть знак зодіаку та період \\(день, тиждень, місяць\\) через пробіл, щоб підписатись на розсилку гороскопа\n\n*Приклад: Овен день*\n\nТи підписаний на розсилку гороскопу для знаку *{notification.Zodiac}* на період *{notification.Period.TranslatePeriodToUkrainian()}*", parseMode: ParseMode.MarkdownV2, replyMarkup: GetCancelKeyboard("овен день", BotCommands.Unsubscribe));
    }
}
