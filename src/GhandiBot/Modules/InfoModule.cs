using System.Threading.Tasks;
using Discord.Commands;

namespace GhandiBot.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        public Task Info() => ReplyAsync("I am GhandiBot");
    }
}