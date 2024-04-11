using BrowserInterop.Extensions;

using System;
using System.Threading.Tasks;

namespace BrowserInterop
{
    /// <summary>
    ///  represents the visual viewport for a given window. For a page containing iframes, each iframe, as well as the containing page, will have a unique window object. Each window on a page will have a unique VisualViewport representing the properties associated with that window.
    /// </summary>
    public class WindowVisualViewPort : JsObjectWrapperBase
    {
        /// <summary>
        /// Returns the offset of the left edge of the visual viewport from the left edge of the layout viewport in CSS pixels.
        /// </summary>
        /// <value></value>
        public double OffsetLeft { get; set; }

        /// <summary>
        /// Returns the offset of the top edge of the visual viewport from the top edge of the layout viewport in CSS pixels.
        /// </summary>
        /// <value></value>
        public double OffsetTop { get; set; }

        /// <summary>
        /// Returns the x coordinate relative to the initial containing block origin of the top edge of the visual viewport in CSS pixels.
        /// </summary>
        /// <value></value>
        public double PageLeft { get; set; }

        /// <summary>
        /// Returns the y coordinate relative to the initial containing block origin of the top edge of the visual viewport in CSS pixels.
        /// </summary>
        /// <value></value>
        public double PageTop { get; set; }

        /// <summary>
        /// Returns the width of the visual viewport in CSS pixels.
        /// </summary>
        /// <value></value>
        public double Width { get; set; }

        /// <summary>
        /// Returns the width of the visual viewport in CSS pixels.
        /// </summary>
        /// <value></value>
        public double Height { get; set; }

        /// <summary>
        /// Returns the pinch-zoom scaling factor applied to the visual viewport.
        /// </summary>
        /// <value></value>
        public double Scale { get; set; }

        /// <summary>
        /// Fired when the visual viewport is resized.
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        public async ValueTask<IAsyncDisposable> OnResize(Func<ValueTask> todo)
        {
            return await JsRuntime.AddEventListenerAsync(JsObjectRef, "", "resize", CallBackInteropWrapper.Create(todo)).ConfigureAwait(false);
        }

        /// <summary>
        /// Fired when the visual viewport is scrolled.
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        public async ValueTask<IAsyncDisposable> OnScroll(Func<ValueTask> todo)
        {
            return await JsRuntime.AddEventListenerAsync(JsObjectRef, "", "scroll", CallBackInteropWrapper.Create(todo)).ConfigureAwait(false);
        }
    }
}