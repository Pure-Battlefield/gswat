using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn
{
    /// <summary>
    /// Represents a parameter for a known packet format.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Generates a parameter from a correct format string.
        /// </summary>
        /// <param name="format">The parameter format.</param>
        public Parameter(string format)
        {
            if (format.Contains('<'))
            {
                IsOptional = false;
            }
            else if (format.Contains('>'))
            {
                IsOptional = true;
            }

            int leftTagIndex = format.IndexOf(IsOptional ? '[' : '<');
            int rightTagIndex = format.IndexOf(IsOptional ? ']' : '>');
            int seperatorIndex = format.IndexOf(':');

            if (seperatorIndex > 0)
            {
                Name = format.Substring(leftTagIndex + 1, seperatorIndex - leftTagIndex - 1).Trim();
                Type = format.Substring(seperatorIndex + 1, format.Length - seperatorIndex - 2).Trim();
            }
            else
            {
                Name = format.Substring(leftTagIndex + 1, rightTagIndex - 1);
                Type = Name;
            }
        }

        public bool IsOptional { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            if (IsOptional)
            {
                return "[" + Name + ":" + Type + "]";
            }
            else 
            {
                return "<" + Name + ":" + Type + ">";
            }
        }
    }
}
