using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GhandiBot.Data.Services;

namespace GhandiBot.Modules
{
    public abstract class DiscordModuleBase
    {
        private readonly FeatureOverrideService _featureOverrideService;
        private readonly DiscordSocketClient _discordSocketClient;

        public DiscordModuleBase(
            FeatureOverrideService featureOverrideService,
            DiscordSocketClient discordSocketClient)
        {
            _featureOverrideService = featureOverrideService
                                      ?? throw new ArgumentNullException(nameof(featureOverrideService));
            _discordSocketClient = discordSocketClient ?? throw new ArgumentNullException(nameof(discordSocketClient));
        }

        public Task<bool> CanExecute(ulong serverId)
        {
            return _featureOverrideService.IsOverriden(GetType().Name, serverId);
        }

        public async Task ReactionAdded(
            ulong serverId,
            Cacheable<IUserMessage, ulong> arg1,
            ISocketMessageChannel arg2,
            SocketReaction arg3)
        {
            if (await CanExecute(serverId)) await ReactionAdded(arg1, arg2, arg3);
        }

        public virtual Task ReactionAdded(
            Cacheable<IUserMessage, ulong> arg1,
            ISocketMessageChannel arg2,
            SocketReaction arg3)
        {
            return Task.CompletedTask;
        }

        public async Task ReactionRemoved(
            ulong serverId,
            Cacheable<IUserMessage, ulong> arg1,
            ISocketMessageChannel arg2,
            SocketReaction arg3)
        {
            if (await CanExecute(serverId)) await ReactionRemoved(arg1, arg2, arg3);
        }

        public virtual Task ReactionRemoved(
            Cacheable<IUserMessage, ulong> arg1,
            ISocketMessageChannel arg2,
            SocketReaction arg3)
        {
            return Task.CompletedTask;
        }

        protected async Task<IDMChannel> GetUser(ulong userId)
        {
            return await _discordSocketClient.GetUser(userId).GetOrCreateDMChannelAsync();
        }
    }
}
