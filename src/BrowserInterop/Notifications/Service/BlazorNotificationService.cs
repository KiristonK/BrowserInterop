using BrowserInterop.Extensions;
using BrowserInterop.Notifications.Enums;
using BrowserInterop.Notifications.Models;

using Microsoft.JSInterop;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
using System.Threading.Tasks;

namespace BrowserInterop.Notifications.Service
{
    internal class BrowserNotificationService(IJSRuntime jSRuntime) : IBrowserNotificationService
    {
        private readonly IJSRuntime JSRuntime = jSRuntime;
        private readonly JsonSerializerSettings JsonSerializerSettings =
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

        /// <inheritdoc/>
        public ValueTask<bool> IsSupportedByBrowserAsync()
        {
            return JSRuntime.InvokeAsync<bool>("eval", new[] { "'Notification' in window" });
        }

        public async ValueTask<bool> IsPermissionGranted()
        {
            return (await JSRuntime.InvokeAsync<string>("eval", new[] { "Notification.permission" })) == "granted";
        }
        public ValueTask<int> MaxActions()
        {
            return JSRuntime.InvokeAsync<int>("eval", new[] { "Notification.maxActions" });
        }

        /// <inheritdoc/>
        public async ValueTask<NotificationPermissionType> RequestPermissionAsync()
        {
            string permission = await JSRuntime.InvokeAsync<string>("Notification.requestPermission");
            if (permission.Equals("granted", StringComparison.InvariantCultureIgnoreCase))
            {
                return NotificationPermissionType.Granted;
            }
            else if (permission.Equals("denied", StringComparison.InvariantCultureIgnoreCase))
            {
                return NotificationPermissionType.Denied;
            }
            else
            {
                return NotificationPermissionType.Default;
            }
        }
        /// <inheritdoc/>
        public async ValueTask<Notification> SendAsync(string title, NotificationOptions options)
        {
            var w = await JSRuntime.GetWindowPropertyRefAsync("window").ConfigureAwait(false);
            var res = await JSRuntime.InvokeInstanceMethodGetWrapper<Notification>(w, "browserInterop.notification.sendNotification", title, options);
            return res;
            //JSRuntime.InvokeAsync<IJSInProcessObjectReference>("eval", new[] { $@"new Notification('{title}', {JsonConvert.SerializeObject(options, JsonSerializerSettings)})" });
        }
        /// <inheritdoc/>
        public ValueTask<Notification> SendAsync(string title, string body, string icon)
        {
            NotificationOptions options = new NotificationOptions
            {
                Body = body,
                Icon = icon,
            };
            return SendAsync(title, options);
            //return JSRuntime.InvokeAsync<IJSInProcessObjectReference>("eval", new[] { $@"new Notification('{title}', {JsonConvert.SerializeObject(options, JsonSerializerSettings)})" });
        }
    }
}
