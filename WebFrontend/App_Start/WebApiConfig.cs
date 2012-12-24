using System.Web.Http;
using System.Web.Mvc;

namespace WebFrontend
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //config.Routes.MapHttpRoute(
            //    name: "GetAllMessages",
            //    routeTemplate: "api/{controller}",
            //    defaults: new { action = "GetAllMessages"}
            //    );
            //config.Routes.MapHttpRoute(
            //    name: "GetMessagesByDay",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new {action = "GetMessagesByDay", id = UrlParameter.Optional}
            //    );
        }
    }
}
