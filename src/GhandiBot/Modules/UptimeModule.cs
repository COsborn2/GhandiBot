using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace GhandiBot.Modules
{
    public class UptimeModule : OverrideableModuleBase<SocketCommandContext>
    {
        private readonly ILogger<UptimeModule> _logger;

        public UptimeModule(ILogger<UptimeModule> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        [Command("uptime")]
        public Task Command()
        {
            var runTime = DateTime.UtcNow - Program.StartTime;

            var formattedTimeSpan =
                $"{runTime.Days} Days, {runTime.Hours} Hours, {runTime.Minutes} Minutes, {runTime.Seconds} Seconds";
            
            return ReplyAsync($"Started at `{Program.StartTime} UTC`. Running for `{formattedTimeSpan}`");
        }
    }
}