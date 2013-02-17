using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn
{
    /// <summary>
    /// A RecognizedPacket is a packet whose format is known.
    /// This lets us pass consumers of Comm a set of key-value 
    /// pairs instead of a set of words.
    /// </summary>
    public class RecognizedPacket
    {
        /// <summary>
        /// Stores the metadata which specifies known request packet formats.
        /// </summary>
        private static Dictionary<string, PacketFormat> ValidRequestFormats = new Dictionary<string,PacketFormat>();

        /// <summary>
        /// Stores the metadata which specified known response packet formats.
        /// Several commands have multiple response formats, hence the List of PacketFormats.
        /// </summary>
        private static Dictionary<string, List<PacketFormat>> ValidResponseFormats = new Dictionary<string,List<PacketFormat>>();


        /// <summary>
        /// Reads the scraped data from BF3_formatted_scrape.txt into the appropriate data structures.
        /// </summary>
        public static void LoadScrapedData() 
        {
            ValidRequestFormats = new Dictionary<string, PacketFormat>();
            ValidResponseFormats = new Dictionary<string, List<PacketFormat>>();

            string[] lines = Resources.BF3_formatted_scrape.Split('\n');
            string currentRequest = "";
            foreach (string line in lines)
            {
                string firstWord = line.Substring(0, line.IndexOf(':'));
                string format = line.Substring(line.IndexOf(':') + 1).Trim();
                PacketFormat formatTemplate = new PacketFormat(format);

                bool isRequest = firstWord == "Request";
                if (isRequest)
                {
                    currentRequest = formatTemplate.Name;
                    ValidRequestFormats[formatTemplate.Name] = formatTemplate;
                }
                if (!isRequest && !ValidResponseFormats.ContainsKey(currentRequest))
                {
                    ValidResponseFormats[currentRequest] = new List<PacketFormat>();
                }

                if (!isRequest)
                {
                    ValidResponseFormats[currentRequest].Add(formatTemplate);
                }
            }
        }
        

        /// <summary>
        /// Names the parameters from the specified request packet.
        /// </summary>
        /// <param name="packet">The request to be formatted.</param>
        /// <returns>Name value pairs of the request's parameters.</returns>
        public static Dictionary<string, string> FormatRequestPacket(Packet packet)
        {
            string name = packet.FirstWord;
            if (packet.IsResponse)
            {
                throw new ArgumentException("The specified packet is not a request packet.");
            }

            if (ValidRequestFormats.ContainsKey(name))
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                PacketFormat format = ValidRequestFormats[name];

                // Is this a special packet, using variable length parameters?
                if (packet.WordCount - 1 > format.Parameters.Count && format.Parameters.Count > 0)
                {
                    parameters[format.Parameters[0].Name] = "";
                    foreach (Word word in packet.Words)
                    {
                        parameters[format.Parameters[0].Name] += new string(word.Content);
                    }

                    return parameters;
                }
                else if (packet.WordCount - 1 > format.Parameters.Count && format.Parameters.Count == 0)
                {
                    throw new ArgumentException("Unknown variable packet format.");
                }

                for (int i = 1; i < packet.WordCount; i++)
                {
                    parameters[format.Parameters[i - 1].Name] = new string(packet.Words[i].Content);
                }

                parameters["packet.Name"] = packet.FirstWord;

                return parameters;
            }

            throw new ArgumentException("Unknown request packet.");
        }


        /// <summary>
        /// Names the parameters from the specified response packet.  The associated request packet
        /// is required to differentiate between response formats -- the response name is not unique across
        /// requests.
        /// </summary>
        /// <param name="request">The request which instigated the response.</param>
        /// <param name="response">The response to be formatted.</param>
        /// <returns>Name value pairs of the response's parameters</returns>
        public static Dictionary<string, string> FormatResponsePacket(Packet request, Packet response)
        {
            string requestName = request.FirstWord;
            if (response.IsRequest)
            {
                throw new ArgumentException("The specified packet is not a response packet.");
            }

            if (!request.IsRequest || request.SequenceNumber != response.SequenceNumber)
            {
                throw new ArgumentException("This request response pair is invalid.");
            }

            if (ValidResponseFormats.ContainsKey(requestName))
            {
                List<PacketFormat> formats = ValidResponseFormats[requestName];

                foreach (PacketFormat format in formats)
                {
                    if (response.FirstWord == format.Name && response.WordCount - 1 >= format.MinimumParameterCount)
                    {
                        Dictionary<string, string> parameters = new Dictionary<string, string>();

                        for (int i = 1; i < response.WordCount; i++)
                        {
                            parameters[format.Parameters[i - 1].Name] = new string(response.Words[i].Content);
                        }

                        parameters["packet.Namae"] = response.FirstWord;
                        return parameters;
                    }
                }
            }

            throw new ArgumentException("Unknown request packet.");
        }


        public static Packet CreatePacketFromFormattedData(string packetName, Dictionary<string, string> parameters)
        {
            if (!ValidRequestFormats.ContainsKey(packetName) || ValidRequestFormats[packetName].MinimumParameterCount > parameters.Count)
            {
                throw new ArgumentException("Invalid packet data.");
            }

            PacketFormat format = ValidRequestFormats[packetName];

            Packet request = new Packet();
            request.OrigininatesFromClient = true;
            request.SequenceNumber = 0; //This will be filled out before being sent.
            List<Word> words = new List<Word>();
            words.Add(new Word(packetName));

            for (int i = 0; i < parameters.Count; i++)
            {
                words.Add(new Word(parameters[format.Parameters[i].Name]));
            }

            request.Words = words;

            return request;
        }
    }
}
