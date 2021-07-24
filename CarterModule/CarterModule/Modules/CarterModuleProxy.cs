using Carter.OpenApi;
using Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CarterModule.Modules
{
    public class CarterModuleProxy : Carter.CarterModule
    {
        public CarterModuleProxy()
        {
        }

        public CarterModuleProxy(string basePath) : base(basePath)
        {
        }

        public void DTORoute<DTO>(Func<HttpRequest, HttpResponse, Task> callback)
        {
            var type = typeof(DTO);
            var types = type.GetCustomAttributes(typeof(CustomDTOAttribute), inherit: false);
            if (types?.Length > 0) {
                var modelAttr = (CustomDTOAttribute)types[0];
                switch (modelAttr.Method) {
                    case "GET":
                        this.Get(modelAttr.Url, callback);
                        break;
                    case "POST":
                        this.Post(modelAttr.Url, callback);
                        break;
                    case "PUT":
                        this.Put(modelAttr.Url, callback);
                        break;
                    case "DELETE":
                        this.Delete(modelAttr.Url, callback);
                        break;
                    default:
                        throw new Exception("DTO Method is invalid : " + type.Name);
                }
            }
        }

        protected new IEndpointConventionBuilder Get(string path, Func<HttpRequest, HttpResponse, Task> handler) => base.Get(path, handler);
        protected new IEndpointConventionBuilder Get(string path, RequestDelegate handler) => base.Get(path, handler);
        protected new IEndpointConventionBuilder Get<T>(string path, Func<HttpRequest, HttpResponse, Task> handler) where T : RouteMetaData => base.Get<T>(path, handler);
        protected new IEndpointConventionBuilder Get<T>(string path, RequestDelegate handler) where T : RouteMetaData => base.Get<T>(path, handler);
        protected new IEndpointConventionBuilder Post(string path, Func<HttpRequest, HttpResponse, Task> handler) => base.Post(path, handler);
        protected new IEndpointConventionBuilder Post(string path, RequestDelegate handler) => base.Post(path, handler);
        protected new IEndpointConventionBuilder Post<T>(string path, Func<HttpRequest, HttpResponse, Task> handler) where T : RouteMetaData => base.Post<T>(path, handler);
        protected new IEndpointConventionBuilder Post<T>(string path, RequestDelegate handler) where T : RouteMetaData => base.Post<T>(path, handler);
        protected new IEndpointConventionBuilder Delete(string path, Func<HttpRequest, HttpResponse, Task> handler) => base.Delete(path, handler);
        protected new IEndpointConventionBuilder Delete(string path, RequestDelegate handler) => base.Delete(path, handler);
        protected new IEndpointConventionBuilder Delete<T>(string path, Func<HttpRequest, HttpResponse, Task> handler) where T : RouteMetaData => base.Delete<T>(path, handler);
        protected new IEndpointConventionBuilder Delete<T>(string path, RequestDelegate handler) where T : RouteMetaData => base.Delete<T>(path, handler);
        protected new IEndpointConventionBuilder Put(string path, Func<HttpRequest, HttpResponse, Task> handler) => base.Put(path, handler);
        protected new IEndpointConventionBuilder Put(string path, RequestDelegate handler) => base.Put(path, handler);
        protected new IEndpointConventionBuilder Put<T>(string path, Func<HttpRequest, HttpResponse, Task> handler) where T : RouteMetaData => base.Put<T>(path, handler);
        protected new IEndpointConventionBuilder Put<T>(string path, RequestDelegate handler) where T : RouteMetaData => base.Put<T>(path, handler);
    }
}
