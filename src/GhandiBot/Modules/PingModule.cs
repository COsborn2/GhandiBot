using System;
using System.Threading.Tasks;
using Discord.Commands;
using GhandiBot.Data;
using Microsoft.Extensions.Logging;

namespace GhandiBot.Modules
{
    public class PingModule : OverrideableModuleBase<SocketCommandContext>
    {
        private readonly ILogger<PingModule> _logger;

        public PingModule(ILogger<PingModule> pingModule)
        {
            _logger = pingModule ?? throw new ArgumentNullException(nameof(pingModule));
        }

        public AppDbContext Con { get; set; }
        
        [Command("ping")]
        public Task Command()
        {
            return ReplyAsync("Pong");
        }
    }
}