using BrowserInterop.IntersectionObserver;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BrowserInterop.Extensions
{
    /// <summary>
    /// Extension to the JSRuntime for using Browser API
    /// </summary>
    public static class BrowserInteropJsRuntimeAsyncExtension
    {
        /// <summary>
        /// Create a WindowInterop instance that can be used for using Browser API
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <returns></returns>
        public static async ValueTask<WindowInterop> WindowAsync(this IJSRuntime jsRuntime)
        {
            var jsObjectRef = await jsRuntime.GetWindowPropertyRefAsync("window").ConfigureAwait(false);
            var wsInterop =
                await jsRuntime.GetInstancePropertyAsync<WindowInterop>(jsObjectRef, "self",
                    WindowInterop.SerializationSpec).ConfigureAwait(false);
            wsInterop.SetJsRuntime(jsRuntime, jsObjectRef);
            return wsInterop;
        }

        /// <summary>
        /// Get the window object property value reference
        /// </summary>
        /// <param name="jsRuntime">current js runtime</param>
        /// <param name="propertyPath">path of the property</param>
        /// <returns></returns>
        public static async ValueTask<IJSObjectReference> GetWindowPropertyRefAsync(this IJSRuntime jsRuntime,
            string propertyPath)
        {
            return await jsRuntime.InvokeAsync<IJSObjectReference>("browserInterop.getPropertyRef", propertyPath).ConfigureAwait(false);
        }

        
        /// <summary>
        /// Get the window object property wrapper for interop calls
        /// </summary>
        /// <param name="jsRuntime">current js runtime</param>
        /// <param name="propertyPath">path of the property</param>
        /// <returns></returns>
        public static async ValueTask<T> GetWindowPropertyWrapperAsync<T>(this IJSRuntime jsRuntime,
            string propertyPath, object serializationSpec) where T : JsObjectWrapperBase
        {
            var objectRef = await GetWindowPropertyRefAsync(jsRuntime, propertyPath).ConfigureAwait(false);
            if (objectRef == null)
            {
                return null;
            }
            var objectContent = await GetInstanceContentAsync<T>(jsRuntime, objectRef, serializationSpec).ConfigureAwait(false);
            
           
            objectContent.SetJsRuntime(jsRuntime, objectRef);
            return objectContent;
        }

        /// <summary>
        /// Get the js object property value
        /// </summary>
        /// <param name="jsRuntime">current js runtime</param>
        /// <param name="propertyPath">path of the property</param>
        /// <param name="jsObjectRef">Ref to the js object from which we'll get the property</param>
        /// <param name="serializationSpec">
        /// An object specifying the member we'll want from the JS object.
        /// "new { allChild = "*", onlyMember = true, ignore = false }" will get all the fields in allChild,
        /// the value of "onlyMember" and will ignore "ignore"
        /// "true" or null will get everything, false will get nothing
        /// </param>        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async ValueTask<T> GetInstancePropertyAsync<T>(this IJSRuntime jsRuntime,
            IJSObjectReference jsObjectRef, string propertyPath, object serializationSpec = null)
        {
            return await jsRuntime.InvokeAsync<T>("browserInterop.getInstancePropertySerializable", jsObjectRef,
                propertyPath, serializationSpec).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the js object property value and initialize its js object reference
        /// </summary>
        /// <param name="jsRuntime">current js runtime</param>
        /// <param name="propertyPath">path of the property</param>
        /// <param name="jsObjectRef">Ref to the js object from which we'll get the property</param>
        /// <param name="serializationSpec">
        /// An object specifying the member we'll want from the JS object.
        /// "new { allChild = "*", onlyMember = true, ignore = false }" will get all the fields in allChild,
        /// the value of "onlyMember" and will ignore "ignore"
        /// "true" or null will get everything, false will get nothing
        /// </param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async ValueTask<T> GetInstancePropertyWrapperAsync<T>(this IJSRuntime jsRuntime,
            IJSObjectReference jsObjectRef, string propertyPath, object serializationSpec = null)
            where T : JsObjectWrapperBase
        {
            var taskContent = GetInstancePropertyAsync<T>(jsRuntime, jsObjectRef, propertyPath, serializationSpec);
            var taskRef = GetInstancePropertyRefAsync(jsRuntime, jsObjectRef, propertyPath);
            var res = await taskContent.ConfigureAwait(false);
            var jsRuntimeObjectRef = await taskRef.ConfigureAwait(false);
            if (res == null)
            {
                return null;
            }
            res.SetJsRuntime(jsRuntime, jsRuntimeObjectRef);
            return res;
        }

        /// <summary>
        /// Set the js object property value
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="jsObjectRef">The JS object you want to change</param>
        /// <param name="propertyPath">The object property name</param>
        /// <param name="value">The new value (can be a IJSObjectReference)</param>
        /// <returns></returns>
        public static async ValueTask SetInstancePropertyAsync(this IJSRuntime jsRuntime, IJSObjectReference jsObjectRef,
            string propertyPath, object value)
        {
            await jsRuntime.InvokeVoidAsync("browserInterop.setInstanceProperty", jsObjectRef, propertyPath, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Return a reference to the JS instance located on the given property 
        /// </summary>
        /// <param name="jsRuntime">Current JS runtime</param>
        /// <param name="jsObjectRef">Reference to the parent instance</param>
        /// <param name="propertyPath">property path</param>
        /// <returns></returns>
        public static async ValueTask<IJSObjectReference> GetInstancePropertyRefAsync(this IJSRuntime jsRuntime,
            IJSObjectReference jsObjectRef, string propertyPath)
        {
            var jsRuntimeObjectRef =
                await jsRuntime.InvokeAsync<IJSObjectReference>("browserInterop.getInstancePropertyRef", jsObjectRef,
                    propertyPath).ConfigureAwait(false);
            return jsRuntimeObjectRef;
        }

        /// <summary>
        /// Call the method on the js instance
        /// </summary>
        /// <param name="jsRuntime">Current JS Runtime</param>
        /// <param name="windowObject">Reference to the JS instance</param>
        /// <param name="methodName">Method name/path </param>
        /// <param name="arguments">method arguments</param>
        /// <returns></returns>
        public static async ValueTask InvokeInstanceMethodAsync(this IJSRuntime jsRuntime, IJSObjectReference windowObject,
            string methodName, params object[] arguments)
        {
            await jsRuntime.InvokeVoidAsync("browserInterop.callInstanceMethod",
                [windowObject, methodName, .. arguments]).ConfigureAwait(false);
        }

        /// <summary>
        /// Call the method on the js instance and return the result
        /// </summary>
        /// <param name="jsRuntime">Current JS Runtime</param>
        /// <param name="windowObject">Reference to the JS instance</param>
        /// <param name="methodName">Method name/path </param>
        /// <param name="arguments">method arguments</param>
        /// <returns></returns>
        public static async ValueTask<T> InvokeInstanceMethodAsync<T>(this IJSRuntime jsRuntime,
            IJSObjectReference windowObject, string methodName, params object[] arguments)
        {
            ArgumentNullException.ThrowIfNull(jsRuntime);

            ArgumentNullException.ThrowIfNull(windowObject);

            return await jsRuntime.InvokeAsync<T>("browserInterop.callInstanceMethod",
                [windowObject, methodName, .. arguments]).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the js object content
        /// </summary>
        /// <param name="jsRuntime">Current JS Runtime</param>
        /// <param name="jsObject">Reference to the JS instance</param>
        /// <param name="serializationSpec">
        /// An object specifying the member we'll want from the JS object.
        /// "new { allChild = "*", onlyMember = true, ignore = false }" will get all the fields in allChild,
        /// the value of "onlyMember" and will ignore "ignore"
        /// "true" or null will get everything, false will get nothing
        /// </param>
        /// <returns></returns>
        public static async ValueTask<T> GetInstanceContentAsync<T>(this IJSRuntime jsRuntime, IJSObjectReference jsObject,
            object serializationSpec)
        {
            return await jsRuntime.InvokeAsync<T>("browserInterop.returnInstance", jsObject, serializationSpec).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the js object content updated
        /// </summary>
        /// <param name="jsRuntime">Current JS Runtime</param>
        /// <param name="jsObject">The JS object for which you want the content updated</param>
        /// <param name="serializationSpec"></param>
        /// <returns></returns>
        public static async ValueTask<T> GetInstanceContentAsync<T>(this IJSRuntime jsRuntime, T jsObject,
            object serializationSpec = null) where T : JsObjectWrapperBase
        {
            ArgumentNullException.ThrowIfNull(jsObject);

            var res = await GetInstanceContentAsync<T>(jsRuntime, jsObject.JsObjectRef, serializationSpec).ConfigureAwait(false);
            res.SetJsRuntime(jsRuntime, jsObject.JsObjectRef);
            return res;
        }


        /// <summary>
        /// Call the method on the js instance and return the reference to the js object
        /// </summary>
        /// <param name="jsRuntime">Current JS Runtime</param>
        /// <param name="jsObject">Reference to the JS instance</param>
        /// <param name="methodName">Method name/path </param>
        /// <param name="arguments">method arguments</param>
        /// <returns></returns>
        public static async ValueTask<IJSObjectReference> InvokeInstanceMethodGetRefAsync(this IJSRuntime jsRuntime,
            IJSObjectReference jsObject, string methodName, params object[] arguments)
        {
            ArgumentNullException.ThrowIfNull(jsRuntime);

            var jsRuntimeObjectRef = await jsRuntime.InvokeAsync<IJSObjectReference>(
                "browserInterop.callInstanceMethodGetRef",
                [jsObject, methodName, .. arguments]).ConfigureAwait(false);
            return jsRuntimeObjectRef;
        }

        /// <summary>
        /// Call the method on the js instance and return the reference to the js object and return wrapper
        /// </summary>
        /// <typeparam name="T">Type of the returned object</typeparam>
        /// <param name="jsRuntime">Current JS Runtime</param>
        /// <param name="jsObject">Reference to the JS instance</param>
        /// <param name="methodName">Method name/path </param>
        /// <param name="arguments">method arguments</param>
        /// <returns></returns>
        public static async ValueTask<T> InvokeInstanceMethodGetWrapper<T>(this IJSRuntime jsRuntime,
           IJSObjectReference jsObject, string methodName, params object[] arguments) where T : JsObjectWrapperBase
        {
            ArgumentNullException.ThrowIfNull(jsRuntime);

            var jsRuntimeObjectRef = await jsRuntime.InvokeAsync<IJSObjectReference>(
                "browserInterop.callInstanceMethodGetRef",
                [jsObject, methodName, .. arguments]).ConfigureAwait(false);
            var res = await jsRuntime.InvokeInstanceMethodAsync<T>(jsRuntimeObjectRef, "valueOf").ConfigureAwait(false);
            res.SetJsRuntime(jsRuntime, jsRuntimeObjectRef);
            return res;
        }


        public static async ValueTask<bool> HasPropertyAsync(this IJSRuntime jsRuntime, IJSObjectReference jsObject,
            string propertyPath)
        {
            return await jsRuntime.InvokeAsync<bool>("browserInterop.hasProperty", jsObject, propertyPath).ConfigureAwait(false);
        }

        /// <summary>
        /// Add an event listener to the given property and event Type
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="jsRuntimeObject"></param>
        /// <param name="propertyName"></param>
        /// <param name="eventName"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static async ValueTask<IAsyncDisposable> AddEventListenerAsync(this IJSRuntime jsRuntime,
            IJSObjectReference jsRuntimeObject, string propertyName, string eventName, CallBackInteropWrapper callBack)
        {
            var listenerId = await jsRuntime.InvokeAsync<int>("browserInterop.addEventListener", jsRuntimeObject,
                propertyName, eventName, callBack).ConfigureAwait(false);

            return new ActionAsyncDisposable(async () =>
                await jsRuntime.InvokeVoidAsync("browserInterop.removeEventListener", jsRuntimeObject, propertyName,
                    eventName, listenerId).ConfigureAwait(false));
        }

        /// <summary>
        /// Invoke the specified method with JSInterop and returns default(T) if the timeout is reached
        /// </summary>
        /// <param name="jsRuntime">js runtime on which we'll execute the query</param>
        /// <param name="identifier">method identifier</param>
        /// <param name="timeout">timeout until e return default(T)</param>
        /// <param name="args">method arguments</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async ValueTask<T> InvokeOrDefaultAsync<T>(this IJSRuntime jsRuntime, string identifier,
            TimeSpan timeout, params object[] args)
        {
            try
            {
                return await jsRuntime.InvokeAsync<T>(identifier,
                    timeout,
                    args).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                //when timeout is reached, it raises an exception
                return await Task.FromResult(default(T)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates an intersection observer <see href="https://developer.mozilla.org/en-US/docs/Web/API/IntersectionObserver/IntersectionObserver"/>
        /// </summary>
        /// <param name="jsRuntime">js runtime on which we'll execute the query</param>
        /// <param name="callback">A function which is called when the percentage of the target element is visible crosses a threshold. The callback receives as input two parameters:<br/>
        /// <br/>
        /// entries<br/>
        /// An array of IntersectionObserverEntry objects, each representing one threshold which was crossed, either becoming more or less visible than the percentage specified by that threshold.<br/>
        /// <br/>
        /// observer<br/>
        /// The IntersectionObserver for which the callback is being invoked.</param>
        /// <param name="options">An optional object which customizes the observer. All properties are optional. You can provide any combination of the following options:</param>
        /// <returns>A new IntersectionObserver which can be used to watch for the visibility of a target element within the specified root crossing through any of the specified visibility thresholds. Call its observe() method to begin watching for the visibility changes on a given target.</returns>
        public static async ValueTask<IntersectionObserverInterop> CreateIntersectionObserver(this IJSRuntime jsRuntime, Func<Task> callback, IntersectionObserverInit options = null)
        {
            var callbackWrapper = CallBackInteropWrapper.Create(async () =>
            {
                //var entries = await entriesRef.InvokeAsync<List<IntersectionObserverEntry>>("valueOf");
                //var observerInterop = await jsRuntime.InvokeInstanceMethodGetWrapper<IntersectionObserverInterop>(observer, "valueOf").ConfigureAwait(false);
                await callback().ConfigureAwait(false);
            }, getJsObjectRef: true);

            var jsObjectRef = await jsRuntime.InvokeAsync<IJSObjectReference>("browserInterop.intersectionObserver.create", callbackWrapper, options).ConfigureAwait(false);

            if (jsObjectRef == null)
                throw new InvalidOperationException("No JS object reference available.");
            var ioInterop = await jsRuntime.InvokeInstanceMethodGetWrapper<IntersectionObserverInterop>(jsObjectRef, "valueOf").ConfigureAwait(false);
            await (await jsRuntime.WindowAsync().ConfigureAwait(false)).Console.Log(ioInterop);
            return ioInterop;
        }
    }
}