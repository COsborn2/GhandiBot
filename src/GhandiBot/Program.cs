using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GhandiBot.Data;
using GhandiBot.Data.Services;
using GhandiBot.Modules;
using GhandiBot.Omdb;
using GhandiBot.Services;
using GhandiBot.Utilities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GhandiBot
{
    class Program
    {
        public static DateTime StartTime { get; private set; }

        public static void Main() => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;
        private HostingEnvironment _environment;

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.development.json", true, true)
                .AddJsonFile("appsettings.connection.json", false, true)
                .AddJsonFile("appsettings.connection.development.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        private IServiceProvider ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            _config = config;

            services.AddSingleton(_client);
            services.AddSingleton<CommandService>();
            services.AddSingleton<CommandHandlingService>();
            services.AddSingleton<HostingEnvironment>();
            _environment = services.BuildServiceProvider().GetRequiredService<HostingEnvironment>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(config);
            });
            services.AddSingleton(config);
            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            services.AddSingleton<OmdbClient>();

            if (_environment.IsProduction())
            {
                const string connectionName = "DefaultConnection";
                var connectionString = config.GetConnectionString(connectionName);

                services.AddDbContextPool<AppDbContext>(options =>
                    options.UseMySql(connectionString));
            }
            else
            {
                var sqliteConnection = new SqliteConnection("DataSource=:memory:");
                sqliteConnection.Open();
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(sqliteConnection);
                });
            }

            services.AddSingleton<FeatureOverrideService>();

            services.AddHttpClient("omdb", c =>
            {
                c.BaseAddress = new Uri("http://www.omdbapi.com/");
            });

            // Register Modules for GuildMemberUpdated event
            var types = Assembly.GetEntryAssembly().GetTypes()
                .Where(x => x.IsSubclassOf(typeof(GuildMemberUpdatedBase)));
            foreach (var type in types)
            {
                services.AddSingleton(type);
            }

            services.AddSingleton<IServiceProvider>(services.BuildServiceProvider());
            return services.BuildServiceProvider();
        }

        private void ConfigureNLog(HostingEnvironment env, string logLocation, string connectionString)
        {
            LogManager.Configuration = new LoggingConfiguration();
            InternalLogger.LogLevel = NLog.LogLevel.Error;
            InternalLogger.LogToConsole = true;

            var jsonLayout = new JsonLayout
            {
                Attributes =
                {
                    new JsonAttribute("time", "${longdate}"),
                    new JsonAttribute("loglevel", "${level:uppercase=true}"),
                    new JsonAttribute("message", "${message}"),
                    new JsonAttribute("exception", "${exception}"),
                    new JsonAttribute("logger", "${logger}"),
                    new JsonAttribute("all-event-properties", "${all-event-properties}")
                },
                EscapeForwardSlash = true
            };

            if (env.IsDevelopment())
            {
                var consoleTarget = new ConsoleTarget
                {
                    Name = "ConsoleLog",
                    Layout = jsonLayout
                };
                LogManager.Configuration.AddTarget(consoleTarget);
                LogManager.Configuration.AddRule(NLog.LogLevel.Trace,
                    NLog.LogLevel.Error,
                    consoleTarget);
            }

            if (env.IsProduction())
            {
                var databaseCommand = @"insert into Logs (Date, Level, Message, CallSite, Exception, Logger) 
                values (
                        @date, @level, @message, @callsite, @exception, @logger
                    )";
                var databaseTarget = new DatabaseTarget
                {
                    Name = "DatabaseLogging",
                    ConnectionString = connectionString,
                    CommandText = databaseCommand,
                    KeepConnection = true,
                    DBProvider = "MySql.Data.MySqlClient.MySqlConnection, MySql.Data",
                    Parameters =
                    {
                        new DatabaseParameterInfo("@date", "${date}"),
                        new DatabaseParameterInfo("@level", "${level}"),
                        new DatabaseParameterInfo("@message", "${message}"),
                        new DatabaseParameterInfo("@callsite", "${callsite-linenumber}"),
                        new DatabaseParameterInfo("@exception", "${exception:tostring}"),
                        new DatabaseParameterInfo("@logger", "${logger}")
                    }
                };

                LogManager.Configuration.AddTarget(databaseTarget);
                LogManager.Configuration.AddRule(NLog.LogLevel.Warn,
                    NLog.LogLevel.Error,
                    databaseTarget);
            }

            var fileTarget = new FileTarget
            {
                Name = "FileLogging",
                ArchiveFileName = new SimpleLayout
                {
                    Text = logLocation+"{####}"
                },
                ArchiveOldFileOnStartup = true,
                ArchiveAboveSize = 10000,
                CreateDirs = true,
                FileName = logLocation,
                Layout = jsonLayout
            };
            LogManager.Configuration.AddTarget(fileTarget);
            LogManager.Configuration.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Error, fileTarget);

            LogManager.ReconfigExistingLoggers();
        }

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _config = BuildConfig();

            var services = ConfigureServices(_config, new ServiceCollection());
            await services
                .GetRequiredService<CommandHandlingService>()
                .InstallCommandsAsync(services);

            services.GetRequiredService<AppDbContext>().Initialize();

            var settings = services.GetRequiredService<IOptions<AppSettings>>().Value;

            var logLocation = settings.LogLocation;
            ConfigureNLog(services.GetRequiredService<HostingEnvironment>(), logLocation,
                _config.GetConnectionString("DefaultConnection"));

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
