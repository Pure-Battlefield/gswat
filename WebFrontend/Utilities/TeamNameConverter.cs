using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFrontend.Utilities
{
    public static class TeamNameConverter
    {
        private static readonly Dictionary<string, string> teams; 
        public static Dictionary<string, string> Teams { get { return teams; } } 

        static TeamNameConverter()
        {
            teams = new Dictionary<string, string>();

            Teams["TEAM1"] = "US";
            Teams["TEAM2"] = "RU";
        }
    }
}