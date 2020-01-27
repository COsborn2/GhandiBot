using System.Threading.Tasks;
using Discord.Commands;

namespace GhandiBot.Modules
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public Task Ping() => ReplyAsync("Pong");
    }
}