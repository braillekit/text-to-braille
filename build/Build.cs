using System;
using System.IO;
using System.Linq;
using EasyBrailleEdit.Common;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    [Solution("Source/EasyBrailleEdit.sln")] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "source";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Executes(() =>
        {
            DeleteDirectories(GlobDirectories(SourceDirectory, "**/bin", "**/obj"));
            DeleteDirectories(GlobDirectories(TestsDirectory, "**/bin", "**/obj"));
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());

            string outputDir = OutputDirectory / "net471";

            //                if (GitRepository.Branch.Equals(Constant.ProductBranches.TaipeiForBlind, StringComparison.CurrentCultureIgnoreCase))
            //                {
            string srcFileName = Path.Combine(outputDir, "AppConfig.ForBlind.ini");
            string dstFileName = Path.Combine(outputDir, "AppConfig.Default.ini");

            Logger.Info(Environment.NewLine + "**********<<< 額外處理 >>>****************");
            Logger.Info($"使用特定分支版本的預設應用程式組態檔：'{Constant.ProductBranches.TaipeiForBlind}'");
            File.Copy(srcFileName, dstFileName, true);
            File.Delete(srcFileName);
            //                }

            // Removing unnecessary files.
            var dir = new DirectoryInfo(outputDir);
            foreach (var file in dir.EnumerateFiles("*.pdb"))
            {
                file.Delete();
            }
        });

    Target Deploy => _ => _
            .DependsOn(Compile)
            .Requires(() => GitVersion != null)
            .Executes(() =>
            {
                var outputDir = OutputDirectory / "net471";
                var updateDir = RootDirectory / "UpdateFiles";

                Logger.Log($"From: {outputDir}");
                Logger.Log($"To: {updateDir}");

                var files = Directory.EnumerateFiles(outputDir, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".exe") || s.EndsWith(".dll")
                        || s.EndsWith(".config") || s.EndsWith(".ini"))
                    .ToList();


                Logger.Log($"Copying {files.Count} files...");

                foreach (var filename in files)
                {
                    string fname = Path.GetFileName(filename);
                    string dstFileName = updateDir / fname;
                    File.Copy(filename, dstFileName, true);

                    Logger.Log($"Copied {fname}");
                }

                string changeLogFile = RootDirectory / "Doc/ChangeLog.txt";
                File.Copy(changeLogFile, updateDir / Path.GetFileName(changeLogFile), true);
                Logger.Log($"\r\nCopied {Path.GetFileName(changeLogFile)}");

                // Don't forget to manually modify Update.txt.
            });
}
