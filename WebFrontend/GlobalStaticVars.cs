using core;
using core.Server;

namespace WebFrontend
{
    public static class GlobalStaticVars
    {
        public static ICore StaticCore { get; set; }
        public static ICommLayer StaticCommLayer { get; set; }
    }
}