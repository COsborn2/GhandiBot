using System;
using System.Threading.Tasks;
using Discord.Commands;
using GhandiBot.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GhandiBot.Modules
{
    public class AllowOverrideAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            CommandInfo command, IServiceProvider services)
        {
            return await services.GetRequiredService<FeatureOverrideService>()
                .IsOverriden(command.Name, context.Guild.Id)
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("This feature has been disabled by the server owner");
        }
    }
}