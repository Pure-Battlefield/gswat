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

            var mockServer = new Mock<IServerMock>();
            var commLayer = new CommLayerMock(mockServer.Object);
            var commHandler = new CommHandler(commLayer);
            GlobalStaticVars.StaticRole = new Core(commHandler);
            ChatMessage msg = new ChatMessage(new DateTime(2012, 12, 18), "Llamautomatic", "This is a test message, generated at server-mock level");
            mockServer.Raise(m => m.MessageSent += null, new ChatEventArgs(msg));
            msg = new ChatMessage(new DateTime(2011, 11, 19), "Webs", "This is another test message");
            mockServer.Raise(m => m.MessageSent += null, new ChatEventArgs(msg));
        }
    }
}