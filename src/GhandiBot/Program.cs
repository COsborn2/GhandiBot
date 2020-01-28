using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GhandiBot
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;
        
        private IServiceProvider ConfigureServices(IConfiguration config)
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                    loggingBuilder.AddNLog(config);
                })
                .AddSingleton(_config)
                .BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        public async Task MainAsync()
        {
            var logger = LogManager.GetCurrentClassLogger();
            
            _client = new DiscordSocketClient();
            _config = BuildConfig();
            
            var target = (FileTarget) LogManager.Configuration.FindTargetByName("FileLog");
            target.FileName = _config["LogLocation"];
            LogManager.ReconfigExistingLoggers();
            
            var services = ConfigureServices(_config);
            await services.GetRequiredService<CommandHandlingService>().InstallCommandsAsync(services);

            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}