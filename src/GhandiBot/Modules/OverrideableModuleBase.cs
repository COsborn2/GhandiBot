using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GhandiBot.Modules
{
    // TODO: Possibly remove this?
    public abstract class OverrideableModuleBase<T> : ModuleBase<T> where T : class, ICommandContext
    {
    }
}
