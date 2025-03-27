using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using System;
using System.Text;
using Core.Exceptions;

namespace Core.Utils
{
    public class ControllerUtil
    {        
        private readonly Controller _controller;
        private readonly IHttpContextAccessor _httpContext_Accessor;

        public ControllerUtil(Controller controller, IHttpContextAccessor httpContext_Accessor)
        {
            _controller = controller;
            _httpContext_Accessor = httpContext_Accessor;
        }

        /// <summary>
        /// Get header from HttpContext.Request.Headers <br/>
        /// return null, if key not found AND throwIfNotExist = false   
        /// </summary>
        /// <param name="key"></param>
        /// <param name="throwIfNotExist"></param>
        /// <returns></returns>
        public string GetHeader(string key, bool throwIfNotExist = true)
        {
            HttpContext httpContext = _httpContext_Accessor.HttpContext;
            if (httpContext == null) throw new BL_Exception($"Unable to access HttpContext");

            StringValues string_Val = StringValues.Empty;
            lock (httpContext)
            {
                if (!httpContext.Request.Headers.TryGetValue(key, out string_Val))
                {
                    if (throwIfNotExist) throw new BL_Exception($"Unable to get http header, Key = \"{key}\"");
                    return null;
                }
            }

            return string_Val.ToString();
        }

        public string HeadersToString(IHeaderDictionary headers)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var header in headers)
            {
                builder.Append($"{header.Key}: {string.Join("|", header.Value)}\r\n");
            }

            return builder.ToString();
        }

        public bool HasRouteData(string key)
        {
            HttpContext httpContext = _httpContext_Accessor.HttpContext;
            if (httpContext == null) throw new BL_Exception($"Unable to access HttpContext");

            bool isContain = false;
            lock (httpContext)
            {
                RouteData rdt = httpContext.GetRouteData();
                isContain = rdt.Values.ContainsKey(key);
            }
            return isContain;
        }

        public bool SetRouteData(string key, object val, bool replace = true)
        {
            HttpContext httpContext = _httpContext_Accessor.HttpContext;
            if (httpContext == null) throw new BL_Exception($"Unable to access HttpContext");

            bool exist = false;
            lock (httpContext)
            {
                RouteValueDictionary routeValues = httpContext.GetRouteData().Values;
                if (routeValues == null) throw new InvalidOperationException("Unable to access HttpContext - Route Data");

                if (replace && routeValues.ContainsKey(key)) routeValues.Remove(key);
                exist = routeValues.TryAdd(key, val);
            }

            return exist;
        }

        public T GetRouteData<T>(string key, bool throwEx = true)
        {
            HttpContext httpContext = _httpContext_Accessor.HttpContext;
            if (httpContext == null) throw new BL_Exception($"Unable to access HttpContext");

            T value = default(T);
            lock (httpContext)
            {
                RouteValueDictionary routeValues = httpContext.GetRouteData().Values;
                if (routeValues == null) throw new InvalidOperationException("Unable to access HttpContext - Route Data");

                object valueObj = routeValues[key]; // if key not found, result null.
                if (valueObj == null)
                {
                    if (throwEx) throw new BL_Exception($"Can't get http RouteData, RouteData key not found or data is null, Key = \"{key}\"");
                    return default(T);
                }

                value = (T)valueObj;
            }

            return value;
        }

        public T GetRouteDataOrDefault<T>(string key)
        {                                    
            T value = GetRouteData<T>(key, throwEx: false);
            return value;
        }

        /// <summary>
        /// Check if header with specified key exist and value is not null or empty.
        /// If not valid then add to <see cref="ControllerBase.ModelState"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message">message to show when validate fail, if not set will use default message.</param>
        public bool ValidateHeaderRequired(string key, string message = "")
        {
            HttpContext httpContext = _httpContext_Accessor.HttpContext;
            if (httpContext == null) throw new BL_Exception($"Unable to access HttpContext");
            
            if (!httpContext.Request.Headers.ContainsKey(key))
            {
                if (string.IsNullOrEmpty(message)) message = $"Header {key} is required.";

                _controller.ModelState.AddModelError(key, message);
                return false;
            }

            return true;
        }
    }
}
