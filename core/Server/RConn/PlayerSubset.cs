namespace core.Server.RConn
{
    /// <summary>
    ///     Player subset
    ///     Several commands – such as admin.listPlayers – take a player subset as argument.
    ///     A player subset is one of the following:
    ///     all        - all players on the server
    ///     team [team number: Team ID]    - all players in the specified team
    ///     squad [team number: Team ID] [squad number: Squad ID] - all players in the specified team+squad
    ///     player [player name: string]     - one specific player
    /// </summary>
    public class PlayerSubset
    {
        public PlayerScope Scope { get; set; }
        public int TeamId { get; set; }
        public int SquadId { get; set; }
        public string Player { get; set; }

        public override string ToString()
        {
            switch (Scope)
            {
                case (PlayerScope.Team):
                    return "team" + TeamId;
                case (PlayerScope.Squad):
                    return "team" + TeamId + "squad" + SquadId;
                case (PlayerScope.Player):
                    return "player" + Player;
                case (PlayerScope.All):
                default:
                    return "all";
            }
        }
    }

    public enum PlayerScope
    {
        All,
        Team,
        Squad,
        Player
    }
}