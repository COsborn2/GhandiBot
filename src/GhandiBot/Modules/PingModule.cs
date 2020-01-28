using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace GhandiBot.Modules
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<PingModule> _logger;

        public PingModule(ILogger<PingModule> pingModule)
        {
            _logger = pingModule ?? throw new ArgumentNullException(nameof(pingModule));
        }

        [Command("ping")]
        public Task Ping()
        {
            return ReplyAsync("Pong");
        }
    }
}