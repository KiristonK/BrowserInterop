using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BrowserInterop.Extensions
{
    /// <summary>
    /// Represents a js object reference, send it to the js interop api and it will be seen as an instance instead of a serialized/deserialized object
    /// </summary>
    public class IJSInProcessObjectReference : IDisposable, IAsyncDisposable
    {
        private bool disposedValue;

        internal IJSRuntime JsRuntime { get; set; }

        [JsonPropertyName("__jsObjectRefId")] public int JsObjectRefId { get; set; }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    (JsRuntime as IJSInProcessRuntime).InvokeVoid("browserInterop.removeObjectRef", JsObjectRefId);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        protected virtual async Task DisposeAsync(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    await JsRuntime.InvokeVoidAsync("browserInterop.removeObjectRef", JsObjectRefId).ConfigureAwait(false);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~IJSInProcessObjectReference()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}