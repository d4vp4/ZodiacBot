using System.Text;
using DataAccess.Enums;
using DataAccess.Repositories.Interfaces;
using Newtonsoft.Json;

namespace NotificationCenter.Jobs;

public class NotificationWorker(IConfiguration configuration, IServiceScopeFactory factory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var scope = factory.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

            var notifications = await repository.GetNotificationsAsync();

            var httpClient = new HttpClient();

            var url = configuration.GetSection("Webhook:Url").Value;

            var bodyString = JsonConvert.SerializeObject(notifications
                .Where(notification => IsPeriod(notification.Period))
                .Select(notification => new { notification.Period, notification.Zodiac, notification.UserId }));

            var content = new StringContent(bodyString, Encoding.UTF8, "application/json");

            _ = Task.Run(() => httpClient.PostAsync(url, content, cancellationToken), cancellationToken);

            await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            // await Task.Delay(TimeSpan.FromHours(24), cancellationToken);
        }
    }

    private static bool IsPeriod(Period period)
    {
        return period switch
        {
            Period.Daily => true,
            Period.Weekly => DateTime.Today.DayOfWeek == DayOfWeek.Monday,
            Period.Monthly => DateTime.Today.Day == 1,
            _ => false
        };
    }
}
