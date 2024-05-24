using System;
using System.Collections.Generic;
using System.Text;

namespace BrowserInterop.Notifications.Models
{
    /// <summary>
    /// Notifications action object.
    /// <br/>
    /// Source: <see href="https://developer.mozilla.org/en-US/docs/Web/API/Notifications/Notifications#actions"/>
    /// </summary>
    public class NotificationAction
    {
        /// <summary>
        /// A string identifying a user action to be displayed on the notification.
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// A string containing action text to be shown to the user.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// A string containing the URL of an icon to display with the action.
        /// </summary>
        public string Icon { get; set; }
    }
}
