using System.Text;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BusinessLogic.Services;

public class NotificationService(IConfiguration configuration) : INotificationService
{
    private string Url => configuration["NotificationCenter:Url"]!;

    public async Task<NotificationDto?> GetNotificationByUserAsync(long userId)
    {
        var response = await HttpWrapper(HttpMethod.Get, urlParams: $"/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var notification = JsonConvert.DeserializeObject<NotificationDto>(content);

            return notification;
        }

        return null;
    }

    public async Task<string?> CreateNotificationAsync(NotificationDto notification)
    {
        var body = JsonConvert.SerializeObject(notification);

        var response = await HttpWrapper(HttpMethod.Post, body: body);

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        return null;
    }

    public async Task<string?> UpdateNotificationAsync(NotificationDto notification)
    {
        var body = JsonConvert.SerializeObject(notification);

        var response = await HttpWrapper(HttpMethod.Put, body: body);

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        return null;
    }

    public async Task<string?> DeleteNotificationAsync(long userId)
    {
        var response = await HttpWrapper(HttpMethod.Delete, urlParams: $"/{userId}");

        if (!response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        return null;
    }

    private async Task<HttpResponseMessage> HttpWrapper(HttpMethod method, string? urlParams = "", string? body = null)
    {
        var client = new HttpClient();

        var request = new HttpRequestMessage(method, Url + urlParams);

        if (body != null)
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);

        return response;
    }
}
