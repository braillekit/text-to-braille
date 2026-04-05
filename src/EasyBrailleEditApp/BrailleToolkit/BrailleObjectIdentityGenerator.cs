using System.Threading;

namespace BrailleToolkit
{
    internal static class BrailleObjectIdentityGenerator
    {
        private static long s_NextWordIdentity;
        private static long s_NextLineIdentity;

        public static long NextWordIdentity()
        {
            return Interlocked.Increment(ref s_NextWordIdentity);
        }

        public static long NextLineIdentity()
        {
            return Interlocked.Increment(ref s_NextLineIdentity);
        }
    }
}
