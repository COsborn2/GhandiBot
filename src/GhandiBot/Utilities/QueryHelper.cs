using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhandiBot.Utilities
{
    public class QueryHelper
    {
        public static Dictionary<string, string> ParseQueryParameters(string requestUri)
        {
            var index = requestUri.IndexOf('?');

            if (index < 0)
            {
                throw new InvalidOperationException("Query parameter not in valid form. Missing leading '?'");
            }
            
            var queryParams = requestUri.Substring(++index);

            var queries = queryParams.Split('&');

            return queries
                .Select(ParseQueryParam)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private static KeyValuePair<string, string> ParseQueryParam(string queryParam)
        {
            var split = queryParam.Split('=');

            if (split.Length != 2)
            {
                throw new InvalidOperationException("One or more query parameters is not in correct form");
            }

            var key = split[0];
            var valueWithSpaces = split[1].Replace("+", " ");
            
            return new KeyValuePair<string, string>(key, valueWithSpaces);
        }
    }
}