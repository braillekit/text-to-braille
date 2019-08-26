using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using EasyBrailleEdit.Common;
using Nuke.Common.IO;

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

    AbsolutePath RealOutputDirectory => RootDirectory / "output" / "net472";

    Target Clean => _ => _
        .Executes(() =>
        {
            GlobDirectories(SourceDirectory, "**/bin", "**/obj").ForEach(DeleteDirectory);
            GlobDirectories(TestsDirectory, "**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Restore"));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Rebuild")
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetMaxCpuCount(Environment.ProcessorCount)
                .SetNodeReuse(IsLocalBuild));

            //                if (GitRepository.Branch.Equals(Constant.ProductBranches.TaipeiForBlind, StringComparison.CurrentCultureIgnoreCase))
            //                {
            string srcFileName = Path.Combine(RealOutputDirectory, "AppConfig.ForBlind.ini");
            string dstFileName = Path.Combine(RealOutputDirectory, "AppConfig.Default.ini");

            Logger.Info(Environment.NewLine + "**********<<< 額外處理 >>>****************");
            Logger.Info($"使用特定分支版本的預設應用程式組態檔：'{Constant.ProductBranches.TaipeiForBlind}'");
            File.Copy(srcFileName, dstFileName, true);
            File.Delete(srcFileName);
            //                }

            // Removing unnecessary files.
            var dir = new DirectoryInfo(RealOutputDirectory);
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
                var updateDir = RootDirectory / "UpdateFiles";

                Logger.Info($"From: {RealOutputDirectory}");
                Logger.Info($"To: {updateDir}");

                var files = Directory.EnumerateFiles(RealOutputDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".exe") || s.EndsWith(".dll")
                        || s.EndsWith(".config") || s.EndsWith(".ini"))
                    .ToList();


                Logger.Info($"Copying {files.Count} files...");

                foreach (var filename in files)
                {
                    string fname = Path.GetFileName(filename);
                    string dstFileName = updateDir / fname;
                    File.Copy(filename, dstFileName, true);

                    Logger.Info($"Copied {fname}");
                }

                string changeLogFile = RootDirectory / "Doc/ChangeLog.txt";
                File.Copy(changeLogFile, updateDir / Path.GetFileName(changeLogFile), true);
                Logger.Info($"\r\nCopied {Path.GetFileName(changeLogFile)}");

                // Don't forget to manually modify Update.txt.
            });
}
