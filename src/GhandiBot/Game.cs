using System;

namespace GhandiBot
{
    public enum Game
    {
        [GameName("Unknown")]
        Unknown = 0,
        
        [GameName("League of Legends")]
        LeagueOfLegends = 1
    }

    public class GameNameAttribute : Attribute
    {
        public string GameName { get; set; }
        public GameNameAttribute(string leagueOfLegends)
        {
            GameName = leagueOfLegends ?? throw new ArgumentNullException(nameof(leagueOfLegends));
        }
    }
}