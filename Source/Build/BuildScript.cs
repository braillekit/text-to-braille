using System;
using FlubuCore.Context;
using FlubuCore.Context.Attributes.BuildProperties;
using FlubuCore.IO;
using FlubuCore.Scripting;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Versioning;

// 注意：需要 FlubuCore v5.1.8 或更新的版本。
namespace _Build
{
    public class BuildScript : DefaultBuildScript
    {
        // 指定建置結果的輸出目錄。
        public FullPath OutputDirectory => RootDirectory.CombineWith("output");

        [ProductId]
        public string ProductId { get; set; } = "EasyBrailleEdit";

        // 指定 .sln 檔案。這裡加上了 "source/"，是因為我把建置專案放在 repository 的跟目錄。
        [SolutionFileName]
        public string SolutionFileName => RootDirectory.CombineWith("Source/EasyBrailleEdit.sln");

        [BuildConfiguration]
        public string BuildConfiguration { get; set; } = "Release"; // Debug or Release        

        //public BuildVersion Version { get; set; }

        protected override void ConfigureTargets(ITaskContext session)
        {
            Console.WriteLine($"輸出目錄為 {OutputDirectory}");

            var clean = session.CreateTarget("clean")
                .SetDescription("Cleaning solution output folder.")
                .AddCoreTask(x => x.Clean()
                    .CleanOutputDir());

            var compile = session.CreateTarget("compile")
                .SetDescription("編譯 solution。")
                .AddCoreTask(x => x.Build());

            var publish = session.CreateTarget("publish")
                .SetDescription("Publish binaries.")
                .DependsOn(compile)
                .AddCoreTask(x => x.Publish()
                    .OutputDirectory(OutputDirectory));
        }
    }
}