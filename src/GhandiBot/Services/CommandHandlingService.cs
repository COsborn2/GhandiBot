using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using GhandiBot;
using GhandiBot.Mixins;
using Microsoft.Extensions.Options;
using Game = GhandiBot.Game;

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
        _client.GuildMemberUpdated += ClientOnGuildMemberUpdated;

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
    }

    private async Task ClientOnGuildMemberUpdated(SocketGuildUser arg1, SocketGuildUser arg2)
    {
        var oldGame = arg1.Activity.GetGame();
        var newGame = arg2.Activity.GetGame();

        // started playing League
        if (oldGame != Game.LeagueOfLegends && newGame == Game.LeagueOfLegends)
        {
            var voiceChannelId = arg1.Guild.VoiceChannels
                .SingleOrDefault(x => x.Name == "Toxic League Players Only")?.Id;
            
            // check to ensure channel doesn't already exist
            if (voiceChannelId is null)
            {
                var categoryId = arg1.Guild.VoiceChannels.First().CategoryId;
                voiceChannelId = (await arg1.Guild.CreateVoiceChannelAsync("Toxic League Players Only", 
                    properties => properties.CategoryId = categoryId)).Id;
            }

            if (arg1.VoiceState.HasValue)
            {
                await arg1.ModifyAsync(properties => properties.ChannelId = voiceChannelId.Value);
                return;
            }
        }
        
        // stopped playing League
        if (oldGame == Game.LeagueOfLegends && newGame != Game.LeagueOfLegends)
        {
            // Move player to first voice channel
            await arg1.ModifyAsync(properties => properties.ChannelId = arg1.Guild.VoiceChannels.First().Id);

            var leagueVoiceChannel = arg2.Guild.VoiceChannels
                .SingleOrDefault(x => x.Name == "Toxic League Players Only");
            if (!(leagueVoiceChannel is null) && !leagueVoiceChannel.Users.Any())
            {
                await arg1.Guild
                    .GetVoiceChannel(arg1.Guild.VoiceChannels.First(x => 
                        x.Name == "Toxic League Players Only").Id)
                    .DeleteAsync();
            }
        }
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