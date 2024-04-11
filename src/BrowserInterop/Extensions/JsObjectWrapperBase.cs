using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;

using System;
using System.Threading.Tasks;

namespace BrowserInterop.Extensions
{
    public abstract class JsObjectWrapperBase : IDisposable, IAsyncDisposable
    {
        public IJSObjectReference JsObjectRef { get; private set; }
        protected internal IJSRuntime JsRuntime { get; private set; }

        /// <summary>
        /// Invoke when internal <see cref="JsObjectRef"/> is disposed but .NET object is not finilized and not marked as disposed yet
        /// </summary>
        public event EventHandler OnBeforeDispose;

        protected internal virtual void SetJsRuntime(IJSRuntime jsRuntime, IJSObjectReference jsObjectRef)
        {
            JsObjectRef = jsObjectRef ?? throw new ArgumentNullException(nameof(jsObjectRef));
            JsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        public async void Dispose()
        {
            OnBeforeDispose?.Invoke(this, EventArgs.Empty);
            await JsObjectRef.DisposeAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
        public async ValueTask DisposeAsync()
        {
            OnBeforeDispose?.Invoke(this, EventArgs.Empty);
            await JsObjectRef.DisposeAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
    }
}