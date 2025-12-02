using BrailleToolkit;

namespace BrailleToolkit.Tests
{
    /// <summary>
    /// This fixture ensures that any non-thread-safe singletons from dependent libraries
    /// (like NChinese's ZhuyinDictionary) are initialized once in a single-threaded context
    /// before any tests start running in parallel.
    /// </summary>
    public class GlobalSetupFixture
    {
        public GlobalSetupFixture()
        {
            // By creating a BrailleProcessor instance here, we trigger the initialization
            // of the ZhuyinReverseConversionProvider and, consequently, the loading of
            // the ZhuyinDictionary singleton.
            BrailleProcessor.CreateInstance();
        }
    }
}
