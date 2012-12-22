using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using core;
using core.Server;
using Moq;
using core.ServerInterface;
using core.ChatMessageUtilities;

namespace WebFrontend
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var commLayer = new CommLayer();
            var commHandler = new CommHandler(commLayer);
            commLayer.Connect(Properties.Settings.Default.ServerIP, 
                                Properties.Settings.Default.ServerPort, 
                                Properties.Settings.Default.ServerPassword);

            GlobalStaticVars.StaticRole = new Core(commHandler);
        }
    }
}