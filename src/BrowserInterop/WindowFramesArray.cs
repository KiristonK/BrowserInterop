using BrowserInterop.Extensions;


using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BrowserInterop
{
    /// <summary>
    /// Give access to the direct sub-frames of the current window.
    /// </summary>
    public class WindowFramesArray
    {
        private readonly IJSObjectReference jsRuntimeObjectRef;
        private readonly IJSRuntime jsRuntime;

        internal WindowFramesArray(IJSObjectReference jsRuntimeObjectRef, IJSRuntime jsRuntime)
        {
            this.jsRuntimeObjectRef = jsRuntimeObjectRef;
            this.jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Give access to the direct sub-frames of the current window.
        /// </summary>
        /// <param name="index">Frame index</param>
        /// <returns></returns>
        public async ValueTask<WindowInterop> Get(int index)
        {
            return await jsRuntime.GetInstancePropertyWrapperAsync<WindowInterop>(jsRuntimeObjectRef, $"frames[{index}]",
                WindowInterop.SerializationSpec).ConfigureAwait(false);
        }

        /// <summary>
        /// Count of direct subframes
        /// </summary>
        /// <returns></returns>
        public async ValueTask<int> Length()
        {
            return await jsRuntime.GetInstancePropertyAsync<int>(jsRuntimeObjectRef, "frames.length").ConfigureAwait(false);
        }
    }
}