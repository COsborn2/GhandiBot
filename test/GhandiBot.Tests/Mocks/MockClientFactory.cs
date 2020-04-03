using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ModuleTests.Mocks
{
    public class MockClientFactory : IHttpClientFactory
    {
        private Dictionary<string, HttpClient> _clientFactories = new Dictionary<string, HttpClient>();

        public HttpClient CreateClient(string name)
        {
            if (_clientFactories.TryGetValue(name, out HttpClient httpClient))
            {
                return httpClient;
            }
            
            throw new InvalidOperationException($"Could not find HttpClient for name: '{name}'");
        }

        public void RegisterHttpClient(string name, HttpClient httpClient)
        {
            _clientFactories.Add(name, httpClient);
        }
    }
}