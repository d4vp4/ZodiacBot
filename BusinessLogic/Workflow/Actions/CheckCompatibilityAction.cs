using BusinessLogic.DTOs;
using BusinessLogic.Utilities;
using DataAccess.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DataAccess.Models.User;

namespace BusinessLogic.Workflow.Actions;

public class CheckCompatibilityAction(ZodiacModel model, ITelegramBotClient client) : IWorkflowAction
{
    public async Task ProcessAsync(User user, Update update)
    {
        var text = update.Message?.Text!;

        var zodiacs = text.Split(" ");

        if (zodiacs.Length != 2)
        {
            await client.SendTextMessageAsync(user.UserId, "Невірний формат введення. Спробуйте ще раз.");
            return;
        }

        var firstZodiac = zodiacs[0].TranslateUkrainianToEnglish();
        var secondZodiac = zodiacs[1].TranslateUkrainianToEnglish();

        if (firstZodiac is null)
        {
            await client.SendTextMessageAsync(user.UserId, $"Невідомий знак зодіаку {zodiacs[0]}. Спробуйте ще раз.");
            return;
        }

        if (secondZodiac is null)
        {
            await client.SendTextMessageAsync(user.UserId, $"Невідомий знак зодіаку {zodiacs[1]}. Спробуйте ще раз.");
            return;
        }

        var firstZodiacModel = model.Signs.First(sign => string.Equals(sign.Name, firstZodiac, StringComparison.OrdinalIgnoreCase));
        var secondZodiacModel = model.Signs.First(sign => string.Equals(sign.Name, secondZodiac, StringComparison.OrdinalIgnoreCase));

        var firstZodiacInUkrainian = firstZodiacModel.Name.TranslateEnglishToUkrainian();
        var secondZodiacInUkrainian = secondZodiacModel.Name.TranslateEnglishToUkrainian();

        var message = $"Сумісні знаки для *{firstZodiacInUkrainian}*:\n{string.Join(", ", firstZodiacModel.Compatible.TranslateEnglishToUkrainian())}\n\n" +
                      $"Сумісні знаки для *{secondZodiacInUkrainian}*:\n{string.Join(", ", secondZodiacModel.Compatible.TranslateEnglishToUkrainian())}\n\nВисновок: *{firstZodiacInUkrainian}* і *{secondZodiacInUkrainian}* ";

        var result = firstZodiacModel.Compatible.Contains(secondZodiacModel.Name) || secondZodiacModel.Compatible.Contains(firstZodiacModel.Name);

        message += result ? "сумісні" : "не сумісні";

        await client.SendTextMessageAsync(user.UserId, message, parseMode: ParseMode.MarkdownV2);
    }
}
