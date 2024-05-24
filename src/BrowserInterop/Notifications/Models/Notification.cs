using System;
using System.Threading.Tasks;

using BrowserInterop.Extensions;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BrowserInterop.Notifications.Models
{
    /// <summary>
    /// A notification is an abstract representation of something that happened, such as the delivery of a message.
    /// </summary>
    public class Notification : JsObjectWrapperBase
    {
        #region Properties
        /// <summary>
        /// Defines a title for the notification, which will be shown at the top of the notification window when it is fired.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; private set; }
        #endregion

        #region Options
       

        /// <summary>
        /// The actions read-only property of the Notifications interface provides the actions available for users to choose from for interacting with the notification.
        /// </summary>
        public NotificationAction[] Actions { get; }
        /// <summary>
        /// a <see cref="string"/> containing the URL of the image used to represent the notification when there is not enough space to display the notification itself.
        /// </summary>
        public string Badge { get; private set; }
        /// <summary>
        /// A <see cref="string"/> representing the body text of the notification, which will be displayed below the title.
        /// </summary>
        public string Body { get; private set; }
        /// <summary>
        /// An <see cref="object"/> with arbitrary data that you want associated with the notification. This can be of any data type.
        /// </summary>
        public object Data { get; private set; }
        /// <summary>
        /// The direction in which to display the notification. It defaults to auto, which just adopts the browser's language setting behavior, but you can override that behaviour by setting values of ltr and rtl (although most browsers seem to ignore these settings.)
        /// </summary>
        public string Dir { get; private set; }
        /// <summary>
        /// A <see cref="string"/> containing the URL of an icon to be displayed in the notification.
        /// </summary>
        public string Icon { get; private set; }
        /// <summary>
        /// A <see cref="string"/> containing the URL of an image to be displayed in the notification.
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// The notification's language, as specified using a DOMString representing a BCP 47 language tag. See the Sitepoint ISO 2 letter language codes page for a simple reference.
        /// </summary>
        public string Lang { get; private set; }
        /// <summary>
        /// The renotify read-only property of the Notifications interface specifies whether the user should be notified after a new notification replaces an old one.
        /// </summary>
        public bool? Renotify { get; private set; }
        /// <summary>
        /// Indicates that a notification should remain active until the user clicks or dismisses it, rather than closing automatically. The default value is false.
        /// </summary>
        public bool? RequireInteraction { get; private set; }
        /// <summary>
        /// The silent read-only property of the Notifications interface specifies whether the notification should be silent, i.e., no sounds or vibrations should be issued, regardless of the device settings. 
        /// </summary>
        public bool? Silent { get; private set; }
        /// <summary>
        /// A <see cref="string"/> containing the URL of an audio file to be played when the notification fires.
        /// </summary>
        public string Sound { get; private set; }
        /// <summary>
        /// A <see cref="string"/> representing an identifying tag for the notification.
        /// </summary>
        public string Tag { get; private set; }
        /// <summary>
        /// A <see cref="DateTime"/> representing the time, in milliseconds since 00:00:00 UTC on 1 January 1970, of the event for which the notification was created.
        /// </summary>
        public DateTime? Timestamp { get; private set; } = DateTime.UtcNow;
        /// <summary>
        /// The vibrate read-only property of the Notifications interface specifies a vibration pattern for the device's vibration hardware to emit when the notification fires. 
        /// </summary>
        public short[] Vibrate { get; set; }
        /// <summary>
        /// noscreen: A Boolean specifying whether the notification firing should enable the device's screen or not.
        /// The default is false, which means it will enable the screen.
        /// </summary>
        public bool? NoScreen { get; private set; }
        /// <summary>
        /// sticky: A Boolean specifying whether the notification should be 'sticky', i.e. not easily clearable by the user.
        /// The default is false, which means it won't be sticky.
        /// </summary>
        public bool? Sticky { get; private set; }
        /// <summary>
        /// The amount of seconds until the notifciation is closed. Default is 5 seconds
        /// </summary>
        public int TimeOut { get; private set; } = 5;


        /// <summary>
        /// fires when a user clicks on notification
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async ValueTask<IAsyncDisposable> OnClick(Func<ValueTask> callback)
        {
            return await JsRuntime.AddEventListenerAsync(
                JsObjectRef, "",
                "onclick",
                CallBackInteropWrapper.Create(
                    async () => { await callback.Invoke().ConfigureAwait(false); },
                    getJsObjectRef: false,
                    serializationSpec: false
                )
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// fires when a there'e error with notification
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async ValueTask<IAsyncDisposable> OnClose(Func<ValueTask> callback)
        {
            return await JsRuntime.AddEventListenerAsync(
                JsObjectRef, "",
                "onclose",
                CallBackInteropWrapper.Create(
                    async () => { await callback.Invoke().ConfigureAwait(false); },
                    getJsObjectRef: false,
                    serializationSpec: false
                )
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// fires when notification is closing
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async ValueTask<IAsyncDisposable> OnError(Func<ValueTask> callback)
        {
            return await JsRuntime.AddEventListenerAsync(
                JsObjectRef, "",
                "onerror",
                CallBackInteropWrapper.Create(
                    async () => { await callback.Invoke().ConfigureAwait(false); },
                    getJsObjectRef: false,
                    serializationSpec: false
                )
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// fires when notification is showing
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async ValueTask<IAsyncDisposable> OnShow(Func<ValueTask> callback)
        {
            return await JsRuntime.AddEventListenerAsync(
                JsObjectRef, "",
                "onshow",
                CallBackInteropWrapper.Create(
                    async () => { await callback.Invoke().ConfigureAwait(false); },
                    getJsObjectRef: false,
                    serializationSpec: false
                )
            ).ConfigureAwait(false);
        }

        #endregion

        #region Constructors    
        public Notification()
        {

        }
        public Notification(string title, NotificationOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException($"{nameof(title)}, cannot be null or empty.");

            this.Title = title;
            this.Options = options;

            if (options == null)
                return;

            this.Timestamp = options.Timestamp;
            this.Dir = options.Dir;
            this.Lang = options.Lang;
            this.Badge = options.Badge;
            this.Body = options.Body; ;
            this.Tag = options.Tag;
            this.Icon = options.Icon;
            this.Image = options.Image;
            this.Data = options.Data;
            this.Renotify = options.Renotify;
            this.RequireInteraction = options.RequireInteraction;
            this.Silent = options.Silent;
            this.Sound = options.Sound;
            this.NoScreen = options.NoScreen;
            this.Sticky = options.Sticky;
            this.TimeOut = options.TimeOut ?? this.TimeOut;
            this.Vibrate = options.Vibrate;
            this.Actions = options.Actions;
        }

        public NotificationOptions Options { get; private set; }
        #endregion
    }
}