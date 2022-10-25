using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BrailleToolkit;

namespace BrailleToolkitBenchmark
{
    //[SimpleJob(RuntimeMoniker.Net60)]
    //[SimpleJob(RuntimeMoniker.Net70)]
    [MemoryDiagnoser]
    [UnicodeConsoleLogger]
    public class BrailleProcessorBenchmarks
    {
        BrailleProcessor processor = BrailleProcessor.GetInstance();

        [Benchmark]
        public string ConvertLineTestChinese()
        {
            var brDoc = new BrailleDocument(processor, 32);
            brDoc.Convert("《湯姆歷險記》（英語：The Adventures of Tom Sawyer）是一部美國著名的兒童文學作品 <分數>1/2</分數>");
            return brDoc.GetAllText();
        }
    }
}
