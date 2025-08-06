using System.IO;
using Serilog;
using System.Reflection;

namespace BrailleToolkit.Tests
{
    internal static class Shared
    {
        private static string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(codeBase);
        }

        private static string testDataPath = Path.Combine(GetAssemblyDirectory(), @"TestData\");
        private static string logFile = Path.Combine(GetAssemblyDirectory(), "log-tests-.txt");

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