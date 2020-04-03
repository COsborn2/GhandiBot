using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GhandiBot.Omdb
{
    public class OmdbClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OmdbClient(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings)
        {
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));

            _httpClient = httpClientFactory.CreateClient("omdb");
            _apiKey = appSettings.Value.OmdbApiKey;
        }

        public async Task<ApiResponse<Movie>> GetMovieData(string movieName, int year = 0)
        {
            if (movieName == null) throw new ArgumentNullException(nameof(movieName));
            
            // Query string parameters
            var queryString = new Dictionary<string, string>
            {
                {"t", movieName},
                {"apiKey", _apiKey}
            };

            if (year != 0)
            {
                queryString.Add("y", year.ToString());
            }

            var request = QueryString.Create(queryString).ToUriComponent();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, request);

            var response = await _httpClient.SendAsync(requestMessage, CancellationToken.None);

            var responseResult = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<Movie>
                {
                    Success = false,
                    Message = "An error occurred"
                };
            }
            
            var movie = JsonSerializer.Deserialize<Movie>(responseResult);
            
            // movie wasn't found correctly
            if (string.IsNullOrWhiteSpace(movie.Title))
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(responseResult);
                
                return new ApiResponse<Movie>
                {
                    Success = false,
                    Message = error.Error
                };
            }
            
            return new ApiResponse<Movie>(movie);
        }
    }
}