using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using BusinessLogic.Utilities;
using BusinessLogic.Workflow;
using DataAccess.Enums;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BusinessLogic.Services;

public class ZodiacService(ITelegramBotClient client, IUserRepository userRepository, IServiceProvider serviceProvider) : IZodiacService
{
    public async Task<IActionResult> ProcessRequestAsync(Update update)
    {
        var message = update.Message;

        if (message is null || message.From?.IsBot != false)
            return new OkResult();

        var user = await userRepository.GetUserAsync(message.From.Id) ?? await userRepository.CreateUserAsync(message.From.Id);

        if (ActivatorUtilities.CreateInstance(serviceProvider, typeof(WorkflowManager)) is not WorkflowManager workflowManager)
            return new BadRequestResult();

        var processed = await workflowManager.ExecuteWorkflowAsync(user, update);

        if (processed)
        {
            await userRepository.UpdateUserAsync();

            return new OkObjectResult("Workflow executed successfully.");
        }

        if (user.NextAction is not null)
        {
            var actionType = Type.GetType("BusinessLogic.Workflow.Actions." + user.NextAction)!;

            var action = ActivatorUtilities.CreateInstance(serviceProvider, actionType) as IWorkflowAction;

            await action?.ProcessAsync(user, update)!;

            await userRepository.UpdateUserAsync();
        }
        else
        {
            await client.SendTextMessageAsync(user.UserId, "Не вдалося обробити команду. Спробуйте ще раз.");
        }

        return new OkObjectResult("Next action executed successfully.");
    }

    public async Task<IActionResult> ProcessNotificationsAsync(List<NotificationDto> notifications)
    {
        foreach (var notification in notifications)
        {
            var message = await GetHoroscopeAsync(notification.Zodiac, notification.Period);

            await client.SendTextMessageAsync(notification.UserId, $"Гороскоп на {notification.Period.TranslatePeriodToUkrainian()} для {notification.Zodiac.TranslateEnglishToUkrainian()}:\n\n{message}");
        }

        return new OkResult();
    }

    public async Task<string?> GetHoroscopeAsync(string zodiac, Period period)
    {
        var httpClient = new HttpClient();

        var url = $"https://horoscope-app-api.vercel.app/api/v1/get-horoscope/{period.ToString().ToLower()}?sign={zodiac}";

        var response = await httpClient.GetAsync(url);

        var content = await response.Content.ReadAsStringAsync();

        var horoscopeResponse = JsonConvert.DeserializeObject<HoroscopeResponse>(content);

        return horoscopeResponse?.Data.HoroscopeData;
    }
}

public class HoroscopeResponse
{
    public JsonData Data { get; set; } = null!;
}

public class JsonData
{
    [JsonProperty("horoscope_data")] public string HoroscopeData { get; set; } = null!;
}
