using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using GhandiBot.Data.Services;

namespace GhandiBot.Modules
{
    public abstract class GuildMemberUpdatedBase
    {
        private readonly FeatureOverrideService _featureOverrideService;

        public GuildMemberUpdatedBase(FeatureOverrideService featureOverrideService)
        {
            _featureOverrideService = featureOverrideService 
                                      ?? throw new ArgumentNullException(nameof(featureOverrideService));
        }
        
        public abstract Task ListenToGuildUserUpdate(SocketGuildUser userBefore,
            SocketGuildUser userAfter);

        public async Task Execute(SocketGuildUser userBefore, SocketGuildUser userAfter)
        {
            var isOverridden = await DetermineOverride(userBefore.Guild.Id);

            if (!isOverridden) await ListenToGuildUserUpdate(userBefore, userAfter);
        }

        public async Task<bool> DetermineOverride(ulong serverId)
        {
            return !IsOverrideable || await _featureOverrideService.IsOverriden(GetType().Name, serverId);
        }

        public abstract bool IsOverrideable { get; }
    }
}