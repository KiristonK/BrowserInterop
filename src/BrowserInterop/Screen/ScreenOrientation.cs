using BrowserInterop.Extensions;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BrowserInterop.Screen
{
    /// <summary>
    ///  provides information about the current orientation of the document.
    /// </summary>
    public class ScreenOrientation
    {
        private IJSRuntime jsRuntime;
        private IJSObjectReference screenRef;

        /// <summary>
        /// Returns the document's current orientation type, one of "portrait-primary", "portrait-secondary", "landscape-primary", or "landscape-secondary".
        /// </summary>
        /// <value></value>
        public string Type { get; set; }

        /// <summary>
        /// Enum value for Type
        /// </summary>
        /// <value></value>
        public ScreenOrientationTypeEnum TypeEnum => Type switch
        {
            "portrait-primary" => ScreenOrientationTypeEnum.PortraitPrimary,
            "portrait-secondary" => ScreenOrientationTypeEnum.PortraitSecondary,
            "landscape-primary" => ScreenOrientationTypeEnum.LandscapePrimary,
            "landscape-secondary" => ScreenOrientationTypeEnum.LandscapeSecondary,
            _ => throw new NotSupportedException("ScreenOrientationTypeEnum: " + Type)
        };

        /// <summary>
        /// returns the document's current orientation angle.
        /// </summary>
        /// <value></value>
        public int Angle { get; set; }

        internal void SetJsRuntime(IJSRuntime jsRuntime, IJSObjectReference screenRef)
        {
            this.jsRuntime = jsRuntime;
            this.screenRef = screenRef;
        }

        /// <summary>
        /// fired when the screen changes orientation.
        /// </summary>
        /// <param name="toDo"></param>
        /// <returns></returns>
        public async ValueTask<IAsyncDisposable> OnChange(Func<ValueTask> toDo)
        {
            return await jsRuntime.AddEventListenerAsync(screenRef, "orientation", "change",
                CallBackInteropWrapper.Create(toDo)).ConfigureAwait(false);
        }

        /// <summary>
        /// Locks the orientation of the containing document to its default orientation
        /// </summary>
        /// <returns></returns>
        public async ValueTask Lock(ScreenOrientationTypeEnum newOrientation)
        {
            await jsRuntime.InvokeInstanceMethodAsync(screenRef, "orientation.lock", newOrientation switch
            {
                ScreenOrientationTypeEnum.Any => "any",
                ScreenOrientationTypeEnum.Natural => "natural",
                ScreenOrientationTypeEnum.Landscape => "landscape",
                ScreenOrientationTypeEnum.Portrait => "portrait",
                ScreenOrientationTypeEnum.PortraitPrimary => "portrait-primary",
                ScreenOrientationTypeEnum.PortraitSecondary => "portrait-secondary",
                ScreenOrientationTypeEnum.LandscapePrimary => "landscape-primary",
                ScreenOrientationTypeEnum.LandscapeSecondary => "landscape-secondary",
                _ => throw new NotSupportedException($"ScreenOrientationTypeEnum: {newOrientation}")
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Unlock the orientation of the containing document 
        /// </summary>
        /// <returns></returns>
        public async ValueTask Unlock()
        {
            await jsRuntime.InvokeInstanceMethodAsync(screenRef, "orientation.unlock").ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Enum values for window.screen.orientation.type
    /// </summary>
    public enum ScreenOrientationTypeEnum
    {
        Any,
        Natural,
        Landscape,
        Portrait,
        PortraitPrimary,
        PortraitSecondary,
        LandscapePrimary,
        LandscapeSecondary
    }
}