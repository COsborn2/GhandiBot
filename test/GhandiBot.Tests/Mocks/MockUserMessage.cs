using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace ModuleTests.Mocks
{
    public class MockUserMessage : IUserMessage
    {
        public MockUserMessage(string message, bool isTTS = false, Embed embed = null)
        {
            Content = message;
            IsTTS = isTTS;
            Embeds = new List<IEmbed>{ embed };
        }

        public ulong Id { get; }
        public DateTimeOffset CreatedAt { get; }
        public Task DeleteAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public MessageType Type { get; }
        public MessageSource Source { get; }
        public bool IsTTS { get; }
        public bool IsPinned { get; }
        public bool IsSuppressed { get; }
        public bool MentionedEveryone { get; }
        public string Content { get; }
        public DateTimeOffset Timestamp { get; }
        public DateTimeOffset? EditedTimestamp { get; }
        public IMessageChannel Channel { get; }
        public IUser Author { get; }
        public IReadOnlyCollection<IAttachment> Attachments { get; }
        public IReadOnlyCollection<IEmbed> Embeds { get; }
        public IReadOnlyCollection<ITag> Tags { get; }
        public IReadOnlyCollection<ulong> MentionedChannelIds { get; }
        public IReadOnlyCollection<ulong> MentionedRoleIds { get; }
        public IReadOnlyCollection<ulong> MentionedUserIds { get; }
        public MessageActivity Activity { get; }
        public MessageApplication Application { get; }
        public MessageReference Reference { get; }

        public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifySuppressionAsync(bool suppressEmbeds, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task PinAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task UnpinAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task CrosspostAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddReactionAsync(IEmote emote, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllReactionsAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllReactionsForEmoteAsync(IEmote emote, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public string Resolve(TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name, TagHandling roleHandling = TagHandling.Name,
            TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
        {
            throw new NotImplementedException();
        }

        public IUserMessage ReferencedMessage { get; }

        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions { get; }
    }
}
