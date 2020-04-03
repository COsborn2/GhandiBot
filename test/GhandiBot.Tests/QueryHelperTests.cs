using System;
using System.Collections.Generic;
using System.Linq;
using GhandiBot.Utilities;
using Xunit;

namespace ModuleTests
{
    public class QueryHelperTests
    {
        [Fact]
        public void ParseQueryParameters_TwoParamsWellFormed_DictionaryReturned()
        {
            var requestUri = "http://www.omdbapi.com/?t=The+Invisible+Man&y=2020&apiKey=someKey";

            var dict = QueryHelper.ParseQueryParameters(requestUri);
            
            Assert.NotNull(dict);

            var dictArray = dict
                .OrderBy(x => x.Key)
                .ToList();
            
            Assert.Equal(3, dictArray.Count);

            Assert.Equal(new KeyValuePair<string, string>("apiKey", "someKey"), dictArray[0]);
            Assert.Equal(new KeyValuePair<string, string>("t", "The Invisible Man"), dictArray[1]);
            Assert.Equal(new KeyValuePair<string, string>("y", "2020"), dictArray[2]);
        }
        
        [Fact]
        public void ParseQueryParameters_MissingQuestionMark_InvalidOperationException()
        {
            var requestUri = "http://www.omdbapi.com/t=The+Invisible+Man&y=2020&apiKey=someKey";

            Assert.Throws<InvalidOperationException>(() => QueryHelper.ParseQueryParameters(requestUri));
        }

        [Fact]
        public void ParseQueryParameters_MalformedQueryParam_InvalidOperationException()
        {
            var requestUri = "http://www.omdbapi.com/?tThe+Invisible+Man&y=2020&apiKey=someKey";

            Assert.Throws<InvalidOperationException>(() => QueryHelper.ParseQueryParameters(requestUri));
        }

        [Fact]
        public void ParseQueryParameters_SingleQueryParameter_DictionaryReturned()
        {
            var requestUri = "http://www.omdbapi.com/?t=The+Invisible+Man";
            
            var dict = QueryHelper.ParseQueryParameters(requestUri);
            
            Assert.NotNull(dict);

            var dictArray = dict
                .OrderBy(x => x.Key)
                .ToList();
            
            Assert.Single(dictArray);

            Assert.Equal(new KeyValuePair<string, string>("t", "The Invisible Man"), dictArray[0]);
        }
    }
}