using System;
using System.Linq;
using System.Reflection;
using Discord;

namespace GhandiBot.Mixins
{
    public static class IActivityMixins
    {
        public static Game GetGame(this IActivity activity)
        {
            var allGames = Enum.GetValues(typeof(Game)).Cast<Game>()
                .Select(game => (game, game.GetAttribute<GameNameAttribute>().GameName));

            var theGame = allGames.SingleOrDefault(x => x.GameName == activity.Name);

            return theGame == default ? Game.Unknown : theGame.game;
        }
    }
}