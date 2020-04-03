using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace GhandiBot.Mixins
{
    public static class SockGuildUserMixins
    {
        public static async Task<IUserMessage> SendEmbedMessage(this SocketGuildUser user, string title, 
            string description, Color color)
        {
            var embed = new EmbedBuilder
            {
                Title = title,
                Description = description,
                Color = color
            };

            return await user.SendMessageAsync(embed: embed.Build());
        }
    }
}