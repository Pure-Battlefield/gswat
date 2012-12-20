using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn
{
    public class Packet
    {

        /// <summary>
        /// Generates an empty packet.
        /// </summary>
        public Packet()
        {
            OriginatesFromServer = false;
            IsRequest = false;
            SequenceNumber = 0;
            Words = new List<Word>();
        }

        /// <summary>
        /// Did this packet originate from the server?
        /// </summary>
        public bool OriginatesFromServer { get; set; }

        /// <summary>
        /// Did this packet originate from the client?
        /// </summary>
        public bool OrigininatesFromClient 
        { 
            get 
            { 
                return !OriginatesFromServer; 
            }
 
            set 
            { 
                OriginatesFromServer = !value; 
            }
        }

        /// <summary>
        /// Is the packet a request in a request - response pair?
        /// </summary>
        public bool IsRequest { get; set; }
        
        /// <summary>
        /// Is the packet a response to a request?
        /// </summary>
        public bool IsResponse { get { return !IsRequest; } set { IsRequest = !value; } }

        private UInt32 _sequenceNumber;

        /// <summary>
        /// Represents the seqence number of this packet for a given connection.
        /// The 2 greatest order bits are reserved as flags.
        /// </summary>
        public UInt32 SequenceNumber 
        { 
            get 
            { 
                return _sequenceNumber & 0x3fffffff; 
            } 

            set 
            { 
                _sequenceNumber = value; 
            }
        }

        /// <summary>
        /// The size of the entire packet in bytes.
        /// </summary>
        public UInt32 Size
        {
            get
            {
                UInt32 byteCount = 0;

                byteCount += 4; //Sequence number bytes
                byteCount += 4; //Size bytes
                byteCount += 4; //Word count bytes

                foreach (Word word in Words)
                {
                    byteCount += (UInt32)word.Emit().Length;
                }

                return byteCount;
            }
        }


        /// <summary>
        /// The number of RConnWords in the packet.
        /// Note this has absolutely no relation to the number of 
        /// machine words in the packet.
        /// </summary>
        public UInt32 WordCount { get { return (UInt32)Words.Count; } }

        /// <summary>
        /// Set of RConnWords that make up the packet.
        /// </summary>
        public List<Word> Words { get; set; }
    }
}
