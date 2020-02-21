using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GhandiBot.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GhandiBot
{
    class Program
    {
        public static DateTime StartTime { get; private set; }
        
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;
        
        private IServiceProvider ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            _config = config;

            services.AddSingleton(_client);
            services.AddSingleton<CommandService>();
            services.AddSingleton<CommandHandlingService>();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(config);
            });
            services.AddSingleton(config);
            services.Configure<AppSettings>(config.GetSection("AppSettings"));
            
            return services.BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.development.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _config = BuildConfig();
            
            var target = (FileTarget) LogManager.Configuration.FindTargetByName("FileLog");
            
            var services = ConfigureServices(_config, new ServiceCollection());
            await services.GetRequiredService<CommandHandlingService>().InstallCommandsAsync(services);

            var settings = services.GetRequiredService<IOptions<AppSettings>>().Value;

            var directory = settings.LogLocation;
            var logLocation = Path.Combine(directory, DateTime.UtcNow.ToLocalTime()
                .ToString("MMddyyyy-hh:mm:ss"));
            target.FileName = logLocation;
            LogManager.ReconfigExistingLoggers();

            var token = settings.Token;
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogDebug($"Token: {token}");

            StartTime = DateTime.UtcNow;
            
            logger.LogDebug($"{StartTime}");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}