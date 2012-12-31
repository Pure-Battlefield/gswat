using System;

namespace core.Server.RConn.Commands
{
    /// <summary>
    ///     Request:  player.onChat [source soldier name: string] [text: string] [target players: player subset]
    ///     Response:  OK
    ///     Effect:   Player with name [source soldier name] (or the server, or the server admin) has sent chat
    ///     message [text] to [target players]
    ///     Comment:  If [source soldier name] is “Server”, then the message was sent from  the server rather than from an
    ///     actual player
    /// </summary>
    internal class OnChat : Packet
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

        public PlayerSubset TargetPlayers
        {
            get
            {
                string result = "";

                if (Words.Count >= 4)
                {
                    for(int i=3; i<Words.Count; i++)
                    result += new string(Words[i].Content);
                }

                if (Words.Count >= 5)
                {
                    for(int i=4; i<Words.Count; i++)
                    result += new string(Words[i].Content);
                }

                PlayerSubset subset = new PlayerSubset();

                if (result.Contains("team"))
                {
                    subset.Scope = PlayerScope.Team;
                    subset.TeamId = Convert.ToInt32(result.Substring(result.Length - 1));
                }
                else if (result.Contains("squad"))
                {
                    subset.Scope = PlayerScope.Squad;
                    subset.TeamId = Convert.ToInt32(result.Substring(result.Length - 2, 1));
                    subset.SquadId = Convert.ToInt32(result.Substring(result.Length - 1));
                }
                else
                {
                    subset.Scope = PlayerScope.All;
                }

                return subset;
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