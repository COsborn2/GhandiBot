using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GhandiBot.Data.Services;
using GhandiBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GhandiBot.Modules
{
    public class FeatureModule : OverrideableModuleBase<SocketCommandContext>
    {
        private readonly ILogger<FeatureModule> _logger;
        private readonly CommandHandlingService _commandHandlingService;
        private readonly IServiceProvider _provider;
        private readonly FeatureOverrideService _featureOverrideService;

        private static Dictionary<object, GuildMemberUpdatedBase> GuildMemberUpdatedBaseTypes;
        private static HashSet<string> CommandsThatCanBeOverriden = new HashSet<string>();

        public FeatureModule(ILogger<FeatureModule> logger, CommandHandlingService commandHandlingService,
            IServiceProvider provider, FeatureOverrideService featureOverrideService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandHandlingService = commandHandlingService 
                                      ?? throw new ArgumentNullException(nameof(commandHandlingService));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _featureOverrideService = featureOverrideService 
                                      ?? throw new ArgumentNullException(nameof(featureOverrideService));

            GuildMemberUpdatedBaseTypes = Assembly.GetEntryAssembly().GetTypes()
                .Where(x => x.IsSubclassOf(typeof(GuildMemberUpdatedBase)))
                .Select(x => _provider.GetRequiredService(x))
                .Select(x => (x, (GuildMemberUpdatedBase)x))
                .ToDictionary(x => x.x, x => x.Item2);

            foreach (var baseType in GuildMemberUpdatedBaseTypes)
            {
                CommandsThatCanBeOverriden.Add(baseType.Key.GetType().Name);
            }
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("feature")]
        [Remarks("Only available to server owner")]
        [Summary("View commands that are enabled/disabled or enable/disable commands")]
        public async Task Command(
            [Summary("(enable|disable|status|list)")]
            string option, 
            [Summary("name of the feature to be enabled/disabled. See list to see names of features")]
            string feature = "")
        {
            if (!CommandsThatCanBeOverriden.Contains(feature) && option != "list")
            {
                await ReplyAsync($"Feature '{feature}' cannot be disabled");
                return;
            }
            
            switch (option)
            {
                case "enable":
                    var remove = await _featureOverrideService.RemoveOverride(feature, Context.Guild.Id);
                    var message = remove
                        ? $"'{feature}' is currently disabled"
                        : $"Is '{feature}' already disabled?";
                    await ReplyAsync(message);
                    break;
                case "disable":
                    await _featureOverrideService.AddOverride(feature,
                        Context.Guild.Id,
                        Context.User.Id);
                    await ReplyAsync($"'{feature}' is currently 'disabled'");
                    break;
                case "status":
                    var status = await _featureOverrideService.IsOverriden(feature, Context.Guild.Id);
                    await ReplyAsync($"'{feature}' is currently '{(status ? "disabled" : "enabled")}'");
                    break;
                case "list":
                    var embedMessage = GetFeatureList();
                    await ReplyAsync(null, false, embedMessage);
                    break;
            }
        }

        private Embed GetFeatureList()
        {
            var embed = new EmbedBuilder()
                .WithColor(Color.Green);
            var features = new List<(string featureName, bool canOverride)>();
            foreach (var command in _commandHandlingService.Commands.Commands)
            {
                var overrideAllowed =
                    command.Preconditions.SingleOrDefault(
                        x => x.GetType() == typeof(AllowOverrideAttribute)) != null;
                features.Add((command.Name, overrideAllowed));
            }
            
            // Guild Member Updated Commands
            foreach (var baseType in GuildMemberUpdatedBaseTypes)
            {
                var val = (baseType.Key.GetType().Name, baseType.Value.IsOverrideable);
                features.Add(val);
                
            }

            string featuresNewlineSep = "";
            string overrides = "";
            foreach (var valueTuple in features)
            {
                featuresNewlineSep += $"{valueTuple.featureName}\n";
                overrides += $"{valueTuple.canOverride}\n";
            }

            embed.AddField("Feature List", featuresNewlineSep, true);
            embed.AddField("Is Override Allowed", overrides, true);

            return embed.Build();
        }
    }
}