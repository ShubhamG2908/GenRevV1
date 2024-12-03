using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Genrev.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config) {
            // Web API configuration and services

            //config.Filters.Add(new ApiAuthorizationAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v1/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = RouteParameter.Optional }
            );
        }
    }
}
