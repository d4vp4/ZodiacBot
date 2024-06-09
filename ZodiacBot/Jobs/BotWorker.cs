using BusinessLogic.Interfaces;
using Telegram.Bot;

namespace ZodiacBot.Jobs;

public class BotWorker(IServiceScopeFactory factory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var offset = 0;

        while (true)
        {
            var scope = factory.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            var service = scope.ServiceProvider.GetRequiredService<IZodiacService>();

            var updates = await client.GetUpdatesAsync(offset, cancellationToken: stoppingToken);

            foreach (var update in updates)
            {
                await service.ProcessRequestAsync(update);

                offset = update.Id + 1;
            }
        }
    }
}
