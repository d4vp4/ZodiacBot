using BusinessLogic.DTOs;
using BusinessLogic.Utilities;
using DataAccess.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DataAccess.Models.User;

namespace BusinessLogic.Workflow.Actions;

public class CheckZodiacByDobAction(ZodiacModel model, ITelegramBotClient client) : IWorkflowAction
{
    public async Task ProcessAsync(User user, Update update)
    {
        var text = update.Message?.Text!;

        if (!DateTime.TryParse(text, out var date))
        {
            await client.SendTextMessageAsync(user.UserId, "Невірний формат дати. Спробуйте ще раз.");
            return;
        }

        var zodiac = model.Signs.FirstOrDefault(sign => sign.Start <= date && sign.End >= date);

        if (zodiac is null)
        {
            await client.SendTextMessageAsync(user.UserId, "Не вдалося визначити знак зодіаку. Спробуйте ще раз.");
            return;
        }

        await client.SendTextMessageAsync(user.UserId, $"Ваш знак зодіаку: *{zodiac.Name.TranslateEnglishToUkrainian()}*", parseMode: ParseMode.MarkdownV2);
        user.NextAction = null;
    }
}
