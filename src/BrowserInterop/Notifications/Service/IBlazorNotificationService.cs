using System.Threading.Tasks;

using BrowserInterop.Notifications.Enums;
using BrowserInterop.Notifications.Models;

using Microsoft.JSInterop;

namespace BrowserInterop.Notifications.Service
{
    public interface IBrowserNotificationService
    {
        /// <summary>
        /// Get's maximum available count of actions.
        /// </summary>
        /// <returns></returns>
        ValueTask<int> MaxActions();

        /// <summary>
        /// Checks if the Notifications' API is Support by the browser.
        /// </summary>
        /// <returns></returns>
        ValueTask<bool> IsSupportedByBrowserAsync();
        /// <summary>
        /// Checks if user has given the permission to show notifications.
        /// </summary>
        /// <returns></returns>
        ValueTask<bool> IsPermissionGranted();
        /// <summary>
        /// Request the user for his permission to send notifications.
        /// </summary>
        /// <returns></returns>
        ValueTask<NotificationPermissionType> RequestPermissionAsync();
        /// <summary>
        /// Send a Notifications with <seealso cref="NotificationOptions"/>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        ValueTask<Notification> SendAsync(string title, NotificationOptions options);
        /// <summary>
        /// Send a Notifications with <paramref name="title"/>, <paramref name="body"/> and <paramref name="iconUrl"/>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="iconUrl"></param>
        /// <returns></returns>
        ValueTask<Notification> SendAsync(string title, string body, string iconUrl);
    }
}