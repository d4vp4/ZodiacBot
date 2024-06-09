using BusinessLogic.Utilities;
using DataAccess.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;
using User = DataAccess.Models.User;

namespace BusinessLogic.Workflow.Actions;

public class GetMoonPhaseAction(ITelegramBotClient client) : IWorkflowAction
{
    private const double SynodicMonthDays = 29.53059;

    public async Task ProcessAsync(User user, Update update)
    {
        var moonPhase = CalculateMoonPhase(DateTime.Today);

        var path = Path.Combine("..", "BusinessLogic", "MoonPhases", $"{moonPhase.ToString()}.jpeg");

        await using var stream = File.Open(path, FileMode.Open);

        var media = new InputFileStream(stream);

        await client.SendPhotoAsync(user.UserId, media, caption: $"Місяць сьогодні в фазі *{moonPhase.TranslateMoonPhaseToUkrainian()}*", parseMode: ParseMode.MarkdownV2);

        user.NextAction = null;
    }

    private static MoonPhase CalculateMoonPhase(DateTime currentDate)
    {
        TimeSpan timeSinceNewMoon = currentDate - new DateTime(2000, 1, 6);
        var daysSinceNewMoon = timeSinceNewMoon.TotalDays;

        var phaseIndex = (int)(daysSinceNewMoon % SynodicMonthDays / SynodicMonthDays * 8);

        phaseIndex = phaseIndex >= 0 ? phaseIndex : 0;
        phaseIndex = phaseIndex < 8 ? phaseIndex : 7;

        return (MoonPhase)phaseIndex;
    }
}
