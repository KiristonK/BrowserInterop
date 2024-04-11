using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BrowserInterop.Extensions
{
    public class CancellableEvent
    {
        private readonly IJSRuntime jsRuntime;
        private readonly IJSObjectReference jsRuntimeObjectRef;

        public CancellableEvent(IJSRuntime jsRuntime, IJSObjectReference jsRuntimeObjectRef)
        {
            this.jsRuntime = jsRuntime;
            this.jsRuntimeObjectRef = jsRuntimeObjectRef;
        }


        /// <summary>
        /// Prevent the event default behavior
        /// </summary>
        /// <returns></returns>
        public async ValueTask PreventDefault()
        {
            await jsRuntime.InvokeInstanceMethodAsync(jsRuntimeObjectRef, "preventDefault").ConfigureAwait(false);
        }
    }
}