namespace GhandiBot.Mixins
{
    public static class StringMixins
    {
        public static int NthOccurrence(this string theString, char toFind, int n)
        {
            if (n < 1) return -1;
            
            int count = 0;
            for (int i = 0; i < theString.Length; i++)
            {
                var cur = theString[i];
                if (cur != toFind) continue;
                
                count++;

                if (count == n)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}