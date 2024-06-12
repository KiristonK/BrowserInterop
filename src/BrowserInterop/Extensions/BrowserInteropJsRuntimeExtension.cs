using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserInterop.Extensions
{
    /// <summary>
    /// Extension to the JSRuntime for using Browser API
    /// </summary>
    public static class BrowserInteropJsRuntimeExtension
    {
        /// <summary>
        /// Create a WindowInterop instance that can be used for using Browser API
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <returns></returns>
        public static WindowInterop Window(this IJSInProcessRuntime jsRuntime)
        {
            var jsObjectRef = jsRuntime.GetWindowPropertyRef("window");
            var wsInterop =
                 jsRuntime.GetInstanceProperty<WindowInterop>(jsObjectRef, "self",
                    WindowInterop.SerializationSpec);
            wsInterop.SetJsRuntime(jsRuntime, jsObjectRef);
            return wsInterop;
        }

        /// <summary>
        /// Get the window object property value reference
        /// </summary>
        /// <param name="jsRuntime">current js runtime</param>
        /// <param name="propertyPath">path of the property</param>
        /// <returns></returns>
        public static IJSObjectReference GetWindowPropertyRef(this IJSInProcessRuntime jsRuntime,
            string propertyPath)
        {
            return  jsRuntime.Invoke<IJSObjectReference>("browserInterop.getPropertyRef", propertyPath);
        }
        
        /// <summary>
        /// Get the window object property wrapper for interop calls
        /// </summary>
        /// <param name="jsRuntime">current js runtime</param>
        /// <param name="propertyPath">path of the property</param>
        /// <returns></returns>
        public static T GetWindowPropertyWrapper<T>(this IJSInProcessRuntime jsRuntime,
            string propertyPath, object serializationSpec) where T : JsObjectWrapperBase
        {
            var objectRef =  GetWindowPropertyRef(jsRuntime, propertyPath);
            if (objectRef == null)
            {
                return null;
            }
            var objectContent =  GetInstanceContent<T>(jsRuntime, objectRef, serializationSpec);
            
           
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
        public static T GetInstanceProperty<T>(this IJSInProcessRuntime jsRuntime,
            IJSObjectReference jsObjectRef, string propertyPath, object serializationSpec = null)
        {
            return jsRuntime.Invoke<T>("browserInterop.getInstancePropertySerializable", jsObjectRef,
                propertyPath, serializationSpec);
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
        public static T GetInstancePropertyWrapper<T>(this IJSInProcessRuntime jsRuntime,
            IJSObjectReference jsObjectRef, string propertyPath, object serializationSpec = null)
            where T : JsObjectWrapperBase
        {
            var taskContent = GetInstanceProperty<T>(jsRuntime, jsObjectRef, propertyPath, serializationSpec);
            var jsRuntimeObjectRef = GetInstancePropertyRef(jsRuntime, jsObjectRef, propertyPath);
            var res =  taskContent;
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
        public static void SetInstanceProperty(this IJSInProcessRuntime jsRuntime, IJSObjectReference jsObjectRef,
            string propertyPath, object value)
        {
             jsRuntime.InvokeVoid("browserInterop.setInstanceProperty", jsObjectRef, propertyPath, value);
        }

        /// <summary>
        /// Return a reference to the JS instance located on the given property 
        /// </summary>
        /// <param name="jsRuntime">Current JS runtime</param>
        /// <param name="jsObjectRef">Reference to the parent instance</param>
        /// <param name="propertyPath">property path</param>
        /// <returns></returns>
        public static IJSObjectReference GetInstancePropertyRef(this IJSInProcessRuntime jsRuntime,
            IJSObjectReference jsObjectRef, string propertyPath)
        {
            var jsRuntimeObjectRef =
                 jsRuntime.Invoke<IJSObjectReference>("browserInterop.getInstancePropertyRef", jsObjectRef,
                    propertyPath);
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
        public static void InvokeInstanceMethod(this IJSInProcessRuntime jsRuntime, IJSObjectReference windowObject,
            string methodName, params object[] arguments)
        {
             jsRuntime.InvokeVoid("browserInterop.callInstanceMethod",
                [windowObject, methodName, .. arguments]);
        }

        /// <summary>
        /// Call the method on the js instance and return the result
        /// </summary>
        /// <param name="jsRuntime">Current JS Runtime</param>
        /// <param name="windowObject">Reference to the JS instance</param>
        /// <param name="methodName">Method name/path </param>
        /// <param name="arguments">method arguments</param>
        /// <returns></returns>
        public static T InvokeInstanceMethod<T>(this IJSInProcessRuntime jsRuntime,
            IJSObjectReference windowObject, string methodName, params object[] arguments)
        {
            ArgumentNullException.ThrowIfNull(jsRuntime);

            ArgumentNullException.ThrowIfNull(windowObject);

            return  jsRuntime.Invoke<T>("browserInterop.callInstanceMethod",
                [windowObject, methodName, .. arguments]);
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
        public static T GetInstanceContent<T>(this IJSInProcessRuntime jsRuntime, IJSObjectReference jsObject,
            object serializationSpec)
        {
            return  jsRuntime.Invoke<T>("browserInterop.returnInstance", jsObject, serializationSpec);
        }

        /// <summary>
        /// Get the js object content updated
        /// </summary>
        /// <param name="jsRuntime">Current JS Runtime</param>
        /// <param name="jsObject">The JS object for which you want the content updated</param>
        /// <param name="serializationSpec"></param>
        /// <returns></returns>
        public static T GetInstanceContent<T>(this IJSInProcessRuntime jsRuntime, T jsObject,
            object serializationSpec = null) where T : JsObjectWrapperBase
        {
            ArgumentNullException.ThrowIfNull(jsObject);

            var res =  GetInstanceContent<T>(jsRuntime, jsObject.JsObjectRef, serializationSpec);
            res.SetJsRuntime(jsRuntime, jsObject.JsObjectRef);
            return res;
        }


        /// <summary>
        /// Call the method on the js instance and return the reference to the js object
        /// </summary>
        /// <param name="jsRuntime">Current JS Runtime</param>
        /// <param name="windowObject">Reference to the JS instance</param>
        /// <param name="methodName">Method name/path </param>
        /// <param name="arguments">method arguments</param>
        /// <returns></returns>
        public static IJSObjectReference InvokeInstanceMethodGetRef(this IJSInProcessRuntime jsRuntime,
            IJSObjectReference windowObject, string methodName, params object[] arguments)
        {
            ArgumentNullException.ThrowIfNull(jsRuntime);

            var jsRuntimeObjectRef =  jsRuntime.Invoke<IJSObjectReference>(
                "browserInterop.callInstanceMethodGetRef",
                [windowObject, methodName, .. arguments]);
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
        public static T InvokeInstanceMethodGetWrapper<T>(this IJSInProcessRuntime jsRuntime,
           IJSObjectReference jsObject, string methodName, params object[] arguments) where T : JsObjectWrapperBase
        {
            ArgumentNullException.ThrowIfNull(jsRuntime);

            var jsRuntimeObjectRef = jsRuntime.Invoke<IJSObjectReference>(
                "browserInterop.callInstanceMethodGetRef",
                [jsObject, methodName, .. arguments]);
            var res = jsRuntime.InvokeInstanceMethod<T>(jsRuntimeObjectRef, "valueOf");
            res.SetJsRuntime(jsRuntime, jsRuntimeObjectRef);
            return res;
        }

        public static bool HasProperty(this IJSInProcessRuntime jsRuntime, IJSObjectReference jsObject,
            string propertyPath)
        {
            return  jsRuntime.Invoke<bool>("browserInterop.hasProperty", jsObject, propertyPath);
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
        public static IDisposable AddEventListener(this IJSInProcessRuntime jsRuntime,
            IJSObjectReference jsRuntimeObject, string propertyName, string eventName, CallBackInteropWrapper callBack)
        {
            var listenerId =  jsRuntime.Invoke<int>("browserInterop.addEventListener", jsRuntimeObject,
                propertyName, eventName, callBack);

            return new ActionDisposable(() =>
                 jsRuntime.InvokeVoid("browserInterop.removeEventListener", jsRuntimeObject, propertyName,
                    eventName, listenerId));
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
        public static T InvokeOrDefault<T>(this IJSInProcessRuntime jsRuntime, string identifier,
            TimeSpan timeout, params object[] args)
        {
            try
            {
                return  jsRuntime.Invoke<T>(identifier,
                    timeout,
                    args);
            }
            catch (TaskCanceledException)
            {
                //when timeout is reached, it raises an exception
                return default;
            }
        }

        /// <summary>
        /// Return the value of a DOMHighResTimeStamp to TimeSpan
        /// </summary>
        /// <param name="timeStamp">value of a DOMHighResTimeStamp</param>
        /// <returns></returns>
        public static TimeSpan HighResolutionTimeStampToTimeSpan(this double timeStamp)
        {
            return TimeSpan.FromTicks((long) timeStamp * 10000);
        }
    }
}