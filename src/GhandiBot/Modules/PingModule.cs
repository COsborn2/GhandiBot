using System;
using System.Threading.Tasks;
using Discord.Commands;
using GhandiBot.Data;
using Microsoft.Extensions.Logging;

namespace GhandiBot.Modules
{
    public class PingModule : OverrideableModuleBase<SocketCommandContext>
    {
        public PingModule()
        { }

        [Command("ping")]
        public Task Command()
        {
            return ReplyAsync("Pong");
        }
    }
}
