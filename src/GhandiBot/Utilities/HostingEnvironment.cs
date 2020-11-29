namespace GhandiBot.Utilities
{
    public class HostingEnvironment
    {
        private static bool _isProduction =
#if DEBUG
            false;
#else
            true;
#endif
        public bool IsProduction() => _isProduction;
        public bool IsDevelopment() => !_isProduction;
    }
}
