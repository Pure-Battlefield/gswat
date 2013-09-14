using System.Web.Http;
using System.Web.Mvc;

namespace WebFrontend
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}/{subresource}/{subresourceid}",
                defaults: new { id = RouteParameter.Optional, subresource = RouteParameter.Optional, subresourceid = RouteParameter.Optional}
            );
        }
    }
}
