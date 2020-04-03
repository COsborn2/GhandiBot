using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GhandiBot.Data.Services;
using GhandiBot.Mixins;

namespace GhandiBot.Modules
{
    public class LeagueReallocation : GuildMemberUpdatedBase
    {
        public LeagueReallocation(FeatureOverrideService featureOverrideService) : base(featureOverrideService)
        {
        }

        private const string VoiceChannelName = "Toxic League Players Only";

        public override async Task ListenToGuildUserUpdate(SocketGuildUser userBefore, SocketGuildUser userAfter)
        {
            var oldGame = userBefore.Activity.GetGame();
            var newGame = userAfter.Activity.GetGame();

            var beforeVoiceChannel = userBefore.VoiceChannel;
            var afterVoiceChannel = userAfter.VoiceChannel;
            if (userBefore.Guild.VoiceChannels.All(x => x.Id != beforeVoiceChannel?.Id) 
                && userAfter.Guild.VoiceChannels.All(x => x.Id != afterVoiceChannel?.Id))
            {
                return;
            }

            // started playing League
            if (oldGame != Game.LeagueOfLegends && newGame == Game.LeagueOfLegends)
            {
                var voiceChannelId = userBefore.Guild.VoiceChannels
                    .SingleOrDefault(x => x.Name == VoiceChannelName)?.Id;

                // check to ensure channel doesn't already exist
                if (voiceChannelId is null)
                {
                    var categoryId = userBefore.Guild.VoiceChannels.First().CategoryId;
                    voiceChannelId = (await userBefore.Guild.CreateVoiceChannelAsync(VoiceChannelName,
                        properties => properties.CategoryId = categoryId)).Id;
                }

                if (userBefore.VoiceState.HasValue)
                {
                    await userBefore.ModifyAsync(properties => properties.ChannelId = voiceChannelId.Value);
                    await userBefore.SendEmbedMessage("Ghandi Says",
                        "I will not let anyone walk through my mind with their dirty feet.", Color.Red);
                    return;
                }
            }

            // stopped playing League
            if (oldGame == Game.LeagueOfLegends && newGame != Game.LeagueOfLegends)
            {
                // Move player to first voice channel
                await userBefore.ModifyAsync(properties => properties.ChannelId =
                    userBefore.Guild.VoiceChannels.First(x => x.Name != VoiceChannelName).Id);
                await userAfter.SendEmbedMessage("Ghandi Says",
                    "If you want to change the Zen Garden, start with yourself", Color.Green);

                var leagueVoiceChannel = userAfter.Guild.VoiceChannels
                    .SingleOrDefault(x => x.Name == VoiceChannelName);
                var firstVoiceChannel = userAfter.Guild.VoiceChannels.First();

                if (!(leagueVoiceChannel is null) && leagueVoiceChannel.Users.All(x =>
                    x.Activity.GetGame() != Game.LeagueOfLegends))
                {
                    // Send all players back to first channel
                    foreach (var user in userBefore.Guild.GetVoiceChannel(leagueVoiceChannel.Id).Users)
                    {
                        await user.ModifyAsync(properties => properties.ChannelId = firstVoiceChannel.Id);
                    }
                }
            }
        }

        public override bool IsOverrideable => true;
    }
}