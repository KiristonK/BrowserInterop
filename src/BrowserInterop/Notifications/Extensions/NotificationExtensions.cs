using BrowserInterop.Notifications.Service;

using Microsoft.Extensions.DependencyInjection;

namespace BrowserInterop.Notifications.Extensions
{
    public static class NotificationExtensions
    {
        public static IServiceCollection AddBrowserNotificationService(this IServiceCollection services)
        {
            return services.AddScoped<IBrowserNotificationService, BrowserNotificationService>();
        }
    }
}
