using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GhandiBot.Omdb;
using Microsoft.Extensions.Logging;

namespace GhandiBot.Modules
{
    public class MovieInfoModule : OverrideableModuleBase<SocketCommandContext>
    {
        private readonly OmdbClient _omdbClient;

        public MovieInfoModule(OmdbClient omdbClient)
        {
            _omdbClient = omdbClient ?? throw new ArgumentNullException(nameof(omdbClient));
        }

        [Command("movie")]
        public async Task Command(string movie, int year = 0)
        {
            var movieInfo = await _omdbClient.GetMovieData(movie, year);
            if (!movieInfo.Success)
            {
                await ReplyAsync("Movie could not be found");
                return;
            }

            var movieData = movieInfo.Object;
            
            var builder = new EmbedBuilder()
                .WithTitle(movieData.Title)
                .WithUrl(GetImdbUrl(movieData.ImdbID))
                .WithColor(Color.Blue)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter(footer => {
                    footer
                        .WithText("Prepared At");
                })
                .WithThumbnailUrl(movieData.Poster)
                .AddField("Plot", movieData.Plot)
                .AddField("Rating", GetRatings(movieData.Ratings))
                .AddField("Box Office", movieData.BoxOffice);

            await ReplyAsync(embed: builder.Build());
        }

        private string GetImdbUrl(string imdbId) => $"https://www.imdb.com/title/{imdbId}/";

        private string GetRatings(ICollection<Rating> ratings)
        {
            var stringBuilder = new StringBuilder();

            foreach (var rating in ratings)
            {
                stringBuilder.AppendLine($"{rating.Source}: {rating.Value}");
            }

            return stringBuilder.ToString().Trim();
        }
    }
}