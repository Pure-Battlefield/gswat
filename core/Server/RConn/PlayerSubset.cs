using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn
{
    /// <summary>
    /// Player subset
    /// Several commands – such as admin.listPlayers – take a player subset as argument.
    /// A player subset is one of the following: 
    /// all        - all players on the server
    /// team [team number: Team ID]    - all players in the specified team
    /// squad [team number: Team ID] [squad number: Squad ID] - all players in the specified team+squad
    /// player [player name: string]     - one specific player
    /// </summary>
    public class PlayerSubset
    {
        public PlayerScope Scope { get; set; }
        public int TeamID { get; set; }
        public int SquadID { get; set; }
        public string Player { get; set; }

        public override string ToString()
        {
            switch (Scope)
            {
                case(PlayerScope.TEAM):
                    return "team" + TeamID;
                case(PlayerScope.SQUAD):
                    return "squad" + TeamID + " " + SquadID;
                case(PlayerScope.PLAYER):
                    return "player" + Player;
                case(PlayerScope.ALL):
                default:
                    return "all";
            }
        }
    }

    public enum PlayerScope
    {
        ALL,
        TEAM,
        SQUAD,
        PLAYER
    }
}
