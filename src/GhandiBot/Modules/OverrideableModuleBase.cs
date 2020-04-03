using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GhandiBot.Modules
{
    public abstract class OverrideableModuleBase<T> : ModuleBase<T> where T : class, ICommandContext
    {
        private ModuleBase<T> theBase { get; }

        public OverrideableModuleBase(ModuleBase<T> moduleBase = null)
        {
            theBase = moduleBase ?? this;
        }

        protected override async Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, 
            Embed embed = null, RequestOptions options = null)
        {
            return await theBase.Context.Channel.SendMessageAsync(message, isTTS, embed, options);
        }
    }
}