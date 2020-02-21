using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace GhandiBot.Modules
{
    public class UptimeModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<UptimeModule> _logger;
        private static DateTime StartTime { get; } = DateTime.UtcNow;

        public UptimeModule(ILogger<UptimeModule> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command("uptime")]
        public Task Uptime()
        {
            var runTime = DateTime.UtcNow - StartTime;

            var formattedTimeSpan =
                $"{runTime.Days} Days, {runTime.Hours} Hours, {runTime.Minutes} Minutes, {runTime.Seconds} Seconds";
            
            return ReplyAsync($"Started at `{StartTime} UTC`. Running for `{formattedTimeSpan}`");
        }
    }
}