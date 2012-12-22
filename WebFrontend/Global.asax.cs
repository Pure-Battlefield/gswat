using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using core;
using core.Server;
using core.ServerInterface;

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

            /*
            var blah = RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blah);
            GlobalStaticVars.StaticRole = new Core(commHandler);
            ChatMessage msg = new ChatMessage(new DateTime(2012, 12, 18), "Llamautomatic", "This is a test message, generated at server-mock level");
            mockServer.Raise(m => m.MessageSent += null, new ChatEventArgs(msg));
            msg = new ChatMessage(new DateTime(2013, 11, 19), "Webs", "This is another test message");
            mockServer.Raise(m => m.MessageSent += null, new ChatEventArgs(msg));
             */
            commLayer.Connect(Properties.Settings.Default.ServerIP, 
                                Properties.Settings.Default.ServerPort, 
                                Properties.Settings.Default.ServerPassword);

            GlobalStaticVars.StaticRole = new Core(commHandler);
        }
    }
}