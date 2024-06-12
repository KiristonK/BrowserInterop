using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BrowserInterop.Extensions;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BrowserInterop.IntersectionObserver
{
    public class IntersectionObserverInterop : JsObjectWrapperBase
    {
        private Lazy<IJSObjectReference> root;

        public IJSObjectReference Root { get; set;  }
        public string RootMargin { get; set; }
        public string ScrollMargin { get; set; }
        public float[] Thresholds { get; set; }
        public bool TrackVisibility { get; set; }
        public int Delay { get; set; }

        public async Task DisconnectAsync()
        {
            await JsRuntime.InvokeInstanceMethodAsync(JsObjectRef, "disconnect");
        }

        public async Task ObserveAsync(ElementReference element)
        {
            await JsRuntime.InvokeInstanceMethodAsync(JsObjectRef, "observe", element);

        }

        public async Task UnobserveAsync(ElementReference element)
        {
            await JsRuntime.InvokeInstanceMethodAsync(JsObjectRef, "unobserve", element);

        }


        public async Task<IEnumerable<IntersectionObserverEntry>> TakeRecordsAsync()
        {
            return await JsRuntime.InvokeInstanceMethodAsync<List<IntersectionObserverEntry>>(JsObjectRef, "takeRecords");
        }

        protected internal override void SetJsRuntime(IJSRuntime jsRuntime, IJSObjectReference jsObjectRef)
        {
            base.SetJsRuntime(jsRuntime, jsObjectRef);
            root = new Lazy<IJSObjectReference>(() => ((IJSInProcessRuntime)jsRuntime).GetInstancePropertyRef(jsObjectRef, "root"));
        }
    }
}
