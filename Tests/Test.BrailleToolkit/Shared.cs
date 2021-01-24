using System.IO;
using NUnit.Framework;
using Serilog;

namespace Test.BrailleToolkit
{
    internal static class Shared
    {
        private static string testDataPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\");
        private static string logFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "log-tests-.txt");

        public static string TestDataPath { get => testDataPath; }
        public static string LogFile { get => logFile; }

        public static void SetupLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(LogFile, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
