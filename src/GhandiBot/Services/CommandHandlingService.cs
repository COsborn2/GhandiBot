using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using GhandiBot.Modules;
using GhandiBot.Utilities;
using Microsoft.Extensions.Options;

namespace GhandiBot.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _client;
        private IServiceProvider _Provider;
        public CommandService Commands { get; }
        private readonly HostingEnvironment _environment;
        private readonly AppSettings _settings;

        public CommandHandlingService(IServiceProvider provider, DiscordSocketClient client, CommandService commands,
            IOptions<AppSettings> settings, HostingEnvironment environment)
        {
            _Provider = provider;
            Commands = commands;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _client = client;
        }
    
        public async Task InstallCommandsAsync(IServiceProvider provider)
        {
            _Provider = provider;
            _client.MessageReceived += HandleCommandAsync;

            // Register GuildMemberUpdated
            var types = Assembly.GetEntryAssembly().GetTypes()
                .Where(x => x.IsSubclassOf(typeof(GuildMemberUpdatedBase)))
                .Select(x => (x, _Provider.GetService(x)));
            foreach (var type in types)
            {
                _client.GuildMemberUpdated += ((GuildMemberUpdatedBase) type.Item2).Execute;
            }

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = (SocketUserMessage) messageParam;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix(_settings.Prefix, ref argPos) || 
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);
        
            var result = await Commands.ExecuteAsync(
                context, 
                argPos,
                _Provider);

            if (_environment.IsDevelopment() && !result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}