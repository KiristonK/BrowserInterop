using BrowserInterop.Extensions;

using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BrowserInterop
{
    public class BeforeUnloadEvent
    {
        private readonly IJSRuntime jsRuntime;
        private readonly IJSObjectReference jsRuntimeObjectRef;

        public BeforeUnloadEvent(IJSRuntime jsRuntime, IJSObjectReference jsRuntimeObjectRef)
        {
            this.jsRuntime = jsRuntime;
            this.jsRuntimeObjectRef = jsRuntimeObjectRef;
        }


        /// <summary>
        /// Prompt the user before quitting
        /// </summary>
        /// <returns></returns>
        public async ValueTask Prompt()
        {
            await jsRuntime.SetInstancePropertyAsync(jsRuntimeObjectRef, "returnValue", false).ConfigureAwait(false);
        }
    }
}