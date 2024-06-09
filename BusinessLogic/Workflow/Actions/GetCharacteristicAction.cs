using BusinessLogic.DTOs;
using BusinessLogic.Utilities;
using DataAccess.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DataAccess.Models.User;

namespace BusinessLogic.Workflow.Actions;

public class GetCharacteristicAction(ZodiacModel model, ITelegramBotClient client) : IWorkflowAction
{
    public async Task ProcessAsync(User user, Update update)
    {
        var text = update.Message?.Text!;

        var zodiac = model.Signs.FirstOrDefault(z => z.Name == text.TranslateUkrainianToEnglish());

        if (zodiac is null)
        {
            await client.SendTextMessageAsync(user.UserId, "Не знайдено такого знаку зодіаку. Спробуйте ще раз.");
            return;
        }

        await client.SendTextMessageAsync(user.UserId, $"Характеристика для *{zodiac.Name.TranslateEnglishToUkrainian()}*:\n\n{zodiac.Description}", parseMode: ParseMode.MarkdownV2);
        user.NextAction = null;
    }
}
