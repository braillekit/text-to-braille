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
            // 測試明眼字內含注音符號、冒號後面跟著"我"、以及引號、句號。
            string line = "ㄅˇ我是誰？　我說：「我是神。」";
            string expected = "ㄅˇ我是誰？　我說：「我是神。」";
            BrailleLine brLine = processor.ConvertLine(line);
            string actual = brLine.ToString();
            if (actual != expected)
            {
                Console.WriteLine($"'{expected}' <> '{actual}'");
            }
            return actual;
        }
    }
}
