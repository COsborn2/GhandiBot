using System;
using System.Net.Http;
using System.Threading.Tasks;
using GhandiBot;
using GhandiBot.Omdb;
using Microsoft.Extensions.Options;
using ModuleTests.Mocks;
using Xunit;

namespace ModuleTests
{
    public class OmdbClientTests
    {
        private readonly MockClientFactory _clientFactory = new MockClientFactory();
        private readonly IOptions<AppSettings> _appSettings;
        
        public OmdbClientTests()
        {
            var messageHandler = new MockOmdbMessageHandler();
            var httpClient = new HttpClient(messageHandler);
            httpClient.BaseAddress = new Uri("http://www.omdbapi.com/");
            
            _clientFactory.RegisterHttpClient("omdb", httpClient);
            _appSettings = Options.Create(new AppSettings
            {
                OdmbApiKey = "someKey"
            });
        }

        [Fact]
        public async Task GetMovieData_MovieIsNull_ArgumentNullException()
        {
            var omdbClient = new OmdbClient(_clientFactory, _appSettings);

            await Assert.ThrowsAsync<ArgumentNullException>(() => omdbClient.GetMovieData(null, 1900));
        }

        [Fact]
        public async Task GetMovieData_MovieDoesNotExist_MovieNotFoundResponse()
        {
            var omdbClient = new OmdbClient(_clientFactory, _appSettings);
            var res = await omdbClient.GetMovieData("This Movie Doesn't Exist", 1900);
            
            Assert.False(res.Success);
        }

        [Fact]
        public async Task GetMovieData_WellFormedMovieInput_MovieDataReturned()
        {
            var omdbClient = new OmdbClient(_clientFactory, _appSettings);
            var res = await omdbClient.GetMovieData("The Invisible Man", 2020);
            
            Assert.True(res.Success);
            Assert.Equal("The Invisible Man", res.Object.Title);
        }
    }
}