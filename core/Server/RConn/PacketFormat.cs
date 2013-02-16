using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn
{
    public class PacketFormat
    {
        /// <summary>
        /// Constructs a PacketFormat object given a specification from the RConn docs.
        /// </summary>
        /// <param name="format">The appropriately formated format string.</param>
        public PacketFormat(string format)
        {
            Regex commandNameRegex = new Regex(@"^.\S+");
            Match commandNameMatch = commandNameRegex.Match(format);
            Name = commandNameMatch.Value;

            Parameters = new List<Parameter>();
            Regex parameterRegex = new Regex("<.+?>");
            MatchCollection parameterMatches = parameterRegex.Matches(format);
            foreach(Match match in parameterMatches) 
            {
                Parameters.Add(new Parameter(match.Value));
            }   
        }


        /// <summary>
        /// The name of this packet format.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// The set of parameter names and types for this format.
        /// </summary>
        public List<Parameter> Parameters { get; set; }


        /// <summary>
        /// Returns the required parameter count.
        /// </summary>
        public int MinimumParameterCount
        {
            get
            {
                int count = 0;
                foreach (Parameter parameter in Parameters)
                {
                    if (!parameter.IsOptional)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
    }
}
