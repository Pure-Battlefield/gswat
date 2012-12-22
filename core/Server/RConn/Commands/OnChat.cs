using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn.Commands
{
    /// <summary>
    /// Request:  player.onChat [source soldier name: string] [text: string] [target players: player subset]
    /// Response:  OK
    /// Effect:   Player with name [source soldier name] (or the server, or the server admin) has sent chat
    /// message [text] to [target players]
    /// Comment:  If [source soldier name] is “Server”, then the message was sent from  the server rather than from an
    /// actual player
    /// </summary>
    class OnChat : Packet
    {
        public OnChat(string soldierName, string text, PlayerScope targetPlayers)
            : base("player.onChat {0} {1} {2}", soldierName, text, targetPlayers.ToString())
        {
        }

        public OnChat(Packet packet)
            : base("player.onChat")
        {
            Words = packet.Words;
        }

        public string SoldierName
        {
            get
            {
                if (Words.Count >= 2)
                {
                    return new string(Words[1].Content);
                }

                return "";
            }
        }

        public string Text
        {
            get
            {
                if (Words.Count >= 3)
                {
                    return new string(Words[2].Content);
                }

                return "";
            }
        }

        public string TargetPlayers
        {
            get
            {
                string result = "";

                if (Words.Count >= 4)
                {
                    result += new string(Words[3].Content);
                }

                if (Words.Count >= 5)
                {
                    result += new string(Words[4].Content);
                }

                return result;
            }
        }

        public static bool IsOnChat(Packet request)
        {
            if (request == null || !request.FirstWord.Equals("player.onChat"))
            {
                return false;
            }

            return true;
        }
    }
}
