using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using BusinessLogic.Utilities;
using DataAccess;
using DataAccess.Repositories.Interfaces;
using DataAccess.Repositories.Operations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace ZodiacBot.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddTelegramBot(this IServiceCollection services, string token)
    {
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(_ =>
        {
            var client = new TelegramBotClient(token);

            List<BotCommand> botCommands =
            [
                new BotCommand { Command = BotCommands.Menu, Description = "Переглянути меню" },
                new BotCommand { Command = BotCommands.Help, Description = "Отримати допомогу" }
            ];

            client.SetMyCommandsAsync(botCommands).GetAwaiter().GetResult();

            return client;
        });
    }

    public static void AddDbContext(this IServiceCollection services)
    {
        services.AddDbContext<ZodiacDbContext>(options => { options.UseSqlServer("name=ConnectionStrings:DefaultConnection"); });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }

    public static void AddZodiacServices(this IServiceCollection services)
    {
        services.AddScoped<IZodiacService, ZodiacService>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddScoped(_ =>
        {
            var fileText = File.ReadAllText(Path.Combine("..", "BusinessLogic", "Resources", "zodiac.json"));

            return JsonConvert.DeserializeObject<ZodiacModel>(fileText) ?? throw new Exception("Zodiacs not found");
        });
    }
}
