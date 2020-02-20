using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace GhandiBot.Modules
{
    public class UptimeModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<UptimeModule> _logger;
        private static DateTime StartTime = DateTime.UtcNow;

        public UptimeModule(ILogger<UptimeModule> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command("uptime")]
        public Task Uptime()
        {
            return ReplyAsync($"Started at '{StartTime.ToLocalTime()}'");
        }
    }
}