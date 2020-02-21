using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using GhandiBot;
using Microsoft.Extensions.Options;

public class CommandHandlingService
{
    private readonly DiscordSocketClient _client;
    private IServiceProvider _Provider;
    private readonly CommandService _commands;
    private readonly AppSettings _settings;

    public CommandHandlingService(IServiceProvider provider, DiscordSocketClient client, CommandService commands,
        IOptions<AppSettings> settings)
    {
        _Provider = provider;
        _commands = commands;
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _client = client;
    }
    
    public async Task InstallCommandsAsync(IServiceProvider provider)
    {
        _Provider = provider;
        _client.MessageReceived += HandleCommandAsync;
        
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        int argPos = 0;

        if (!(message.HasCharPrefix(_settings.Prefix, ref argPos) || 
            message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        var context = new SocketCommandContext(_client, message);
        
        var result = await _commands.ExecuteAsync(
            context, 
            argPos,
            _Provider);

        if (!result.IsSuccess)
        {
            await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}