using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFrontend.Utilities
{
    public static class SquadNameConverter
    {
        private static readonly Dictionary<string, string> squads; 
        public static Dictionary<string, string> Squads { get { return squads; } } 

        static SquadNameConverter()
        {
            squads = new Dictionary<string, string>();

            Squads.Add("SQUAD0", " UNSQUADDED");
            Squads.Add("SQUAD1", " ALPHA");
            Squads.Add("SQUAD2", " BRAVO");
            Squads.Add("SQUAD3", " CHARLIE");
            Squads.Add("SQUAD4", " DELTA");
            Squads.Add("SQUAD5", " ECHO");
            Squads.Add("SQUAD6", " FOXTROT");
            Squads.Add("SQUAD7", " GOLF");
            Squads.Add("SQUAD8", " HOTEL");
            Squads.Add("SQUAD9", " INDIA");
            Squads.Add("SQUAD10", " JULIET");
            Squads.Add("SQUAD11", " KILO");
            Squads.Add("SQUAD12", " LIMA");
            Squads.Add("SQUAD13", " MIKE");
            Squads.Add("SQUAD14", " NOVEMBER");
            Squads.Add("SQUAD15", " OSCAR");
            Squads.Add("SQUAD16", " PAPA");
            Squads.Add("SQUAD17", " QUEBEC");
            Squads.Add("SQUAD18", " ROMEO");
            Squads.Add("SQUAD19", " SIERRA");
            Squads.Add("SQUAD20", " TANGO");
            Squads.Add("SQUAD21", " UNIFORM");
            Squads.Add("SQUAD22", " VICTOR");
            Squads.Add("SQUAD23", " WHISKEY");
            Squads.Add("SQUAD24", " XRAY");
            Squads.Add("SQUAD25", " YANKEE");
            Squads.Add("SQUAD26", " ZULU");
            Squads.Add("SQUAD27", " HAGGARD");
            Squads.Add("SQUAD28", " SWEETWATER");
            Squads.Add("SQUAD29", " PRESTON");
            Squads.Add("SQUAD30", " REDFORD");
            Squads.Add("SQUAD31", " FAITH");
            Squads.Add("SQUAD32", " CELESTE");
        }
    }
}