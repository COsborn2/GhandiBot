using GhandiBot.Mixins;
using Xunit;

namespace ModuleTests
{
    public class StringMixinsTests
    {
        [Fact]
        public void NthOccurrence_StringWithMultipleOfChar_IndexReturned()
        {
            string testString = "ababab";
            
            Assert.Equal(3, testString.NthOccurrence('b', 2));
        }

        [Fact]
        public void NthOccurrence_OccurrenceIsFirstChar_IndexReturned()
        {
            string testString = "cbcbcb";
            
            Assert.Equal(0, testString.NthOccurrence('c', 1));
        }

        [Fact]
        public void NthOccurrence_OccurrenceIsLastChar_IndexReturned()
        {
            string testString = "cbcbcb";
            
            Assert.Equal(5, testString.NthOccurrence('b', 3));
        }

        [Fact]
        public void NthOccurrence_StringIsSingleCharacter_IndexReturned()
        {
            string testString = "b";
            
            Assert.Equal(0, testString.NthOccurrence('b', 1));
        }

        [Fact]
        public void NthOccurrence_DoesNotContainChar_Negative1()
        {
            string testString = "abcdefghijk";
            
            Assert.Equal(-1, testString.NthOccurrence('o', 1));
        }

        [Fact]
        public void NthOccurrence_EmptyString_Negative1()
        {
            string testString = "";
            
            Assert.Equal(-1, testString.NthOccurrence('0', 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(12)]
        public void NthOccurrence_BadNValues_Negative1(int n)
        {
            string testString = "abcdefg";
            
            Assert.Equal(-1, testString.NthOccurrence('o', n));
        }
    }
}