using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GhandiBot.Mixins;
using GhandiBot.Utilities;

namespace ModuleTests.Mocks
{
    public class MockOmdbMessageHandler : HttpMessageHandler
    {
        private const string TheInvisibleMan2020Raw =
            "{\"Title\":\"The Invisible Man\",\"Year\":\"2020\",\"Rated\":\"R\"" +
            ",\"Released\":\"28 Feb 2020\",\"Runtime\":\"124 min\"," +
            "\"Genre\":\"Horror, Mystery, Sci-Fi, Thriller\"," +
            "\"Director\":\"Leigh Whannell\",\"Writer\":\"Leigh Whannell " +
            "(screenplay), Leigh Whannell (screen story)\",\"Actors\":\"" +
            "Elisabeth Moss, Oliver Jackson-Cohen, Harriet Dyer, Aldis " +
            "Hodge\",\"Plot\":\"When Cecilia's abusive ex takes his own life " +
            "and leaves her his fortune, she suspects his death was a hoax. As a " +
            "series of coincidences turn lethal, Cecilia works to prove that she is " +
            "being hunted by someone nobody can see.\",\"Language\":\"English\"" +
            ",\"Country\":\"Australia, USA, Canada, UK\",\"Awards\":\"4 nominations." +
            "\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BZjFhM2I4ZD" +
            "YtZWMwNC00NTYzLWE3MDgtNjgxYmM3ZWMxYmVmXkEyXkFqcGdeQXVyMTkxNjUyNQ@@._V1" +
            "_SX300.jpg\",\"Ratings\":[{\"Source\":\"Internet Movie Database\"," +
            "\"Value\":\"7.3/10\"},{\"Source\":\"Metacritic\",\"Value\":\"71/100\"}]," +
            "\"Metascore\":\"71\",\"imdbRating\":\"7.3\",\"imdbVotes\":\"44,708\"," +
            "\"imdbID\":\"tt1051906\",\"Type\":\"movie\",\"DVD\":\"N/A\"," +
            "\"BoxOffice\":\"N/A\",\"Production\":\"Indie Rights\",\"Website\":" +
            "\"N/A\",\"Response\":\"True\"}";

        private const string BadResponseRaw = "{\"Response\":\"False\",\"Error\":\"Movie not found!\"}";
        
        private Dictionary<(string title, int year), string> KnownMovies 
            = new Dictionary<(string title, int year), string>()
            {
                {("The Invisible Man", 2020), TheInvisibleMan2020Raw}
            };

        private readonly HttpResponseMessage BadResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(BadResponseRaw)
        };
        
        private HttpResponseMessage GetResponseMessage(string name, string year)
        {
            if (!int.TryParse(year, out int movieYear))
            {
                return BadResponse;
            }
            
            if (KnownMovies.TryGetValue((name, movieYear), out string rawResponse))
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(rawResponse)
                };
            }

            return BadResponse;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var requestUrl = request.RequestUri.ToString();

            var indexOfSlash = requestUrl.IndexOf('/');

            if (requestUrl.StartsWith("http://") || requestUrl.StartsWith("https://"))
            {
                indexOfSlash = requestUrl.NthOccurrence('/', 3);
            }
            
            var baseUrl = new string(requestUrl.Take(indexOfSlash).ToArray());
            var queryParameters = QueryHelper.ParseQueryParameters(requestUrl);

            switch (baseUrl.ToLower())
            {
                case "http://www.omdbapi.com":
                    var title = queryParameters
                        .FirstOrDefault(x => x.Key == "t")
                        .Value;
                    var year = queryParameters
                        .FirstOrDefault(x => x.Key == "y")
                        .Value;
                    return Task.FromResult(GetResponseMessage(title, year));
                default:
                    return null;
            }
        }
    }
}