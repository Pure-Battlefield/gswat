using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn
{
    public class Word
    {
        /// <summary>
        /// Creates an empty Word.
        /// </summary>
        public Word()
        {
            Content = new char[0];
        }

        /// <summary>
        /// Generates a word from the specified string.
        /// Drops any null terminators.
        /// </summary>
        /// <param name="str">The string to generate the word from.</param>
        public Word(string str)
        {
            Content = str.ToCharArray().Where(c => c != '\0').ToArray();
        }

        /// <summary>
        /// The number of bytes in this RConnWord.
        /// Note the typecast is OK since no packet can be larger than 
        /// 16384 bytes.
        /// </summary>
        public UInt32 Size { get { return (UInt32)Content.Length; } }

        /// <summary>
        /// The bytes containing data, this byte cannot contain a null-byte.
        /// A null byte terminates this array upon being emitted.
        /// </summary>
        public char[] Content { get; set; }
    }
}
