using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GhandiBot.Data.Services;

namespace GhandiBot.Modules
{
    public class EventSchedulerModule : DiscordModuleBase
    {
        public EventSchedulerModule(
            FeatureOverrideService featureOverrideService,
            DiscordSocketClient discordSocketClient)
            : base(featureOverrideService, discordSocketClient)
        {
        }

        public override async Task ReactionAdded(
            Cacheable<IUserMessage, ulong> arg1,
            ISocketMessageChannel arg2,
            SocketReaction arg3)
        {
            var userDmChannel = await GetUser(arg3.UserId);
            await userDmChannel.SendMessageAsync("You have joined the event");
        }

        public override Task ReactionRemoved(
            Cacheable<IUserMessage, ulong> arg1,
            ISocketMessageChannel arg2,
            SocketReaction arg3)
        {
            return base.ReactionRemoved(arg1, arg2, arg3);
        }
    }
}
