using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GhandiBot.Modules;

namespace ModuleTests.Mocks
{
    public class MockModuleBase<T> : OverrideableModuleBase<T> where T : class, ICommandContext
    {
        public List<MockUserMessage> SentMessages { get; } = new List<MockUserMessage>();

        protected override Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null,
            AllowedMentions allowedMentions = null, MessageReference messageReference = null)
        {
            var mockMessage = new MockUserMessage(message, isTTS, embed);

            var tcs = new TaskCompletionSource<IUserMessage>();
            try
            {
                tcs.SetResult(mockMessage);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }

            return tcs.Task;
        }
    }
}
