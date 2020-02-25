using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace GhandiBot.Modules
{
    public class InfoModule : OverrideableModuleBase<SocketCommandContext>
    {
        private readonly ILogger<InfoModule> _logger;

        [Command("info")]
        public Task Info() => ReplyAsync("I am GhandiBot");

        public InfoModule(ILogger<InfoModule> logger)  
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}