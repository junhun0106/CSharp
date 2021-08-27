using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using IntParserLib;
using BoolParseLib;

namespace IntParse
{
    [MemoryDiagnoser]
    public class BoolParseBenchMark
    {
        private const string testString = "true";

        [Benchmark]
        public void BoolParse()
        {
            // 파라미터로 span 객체가 넘어 왔다고 가정 한다
            var span = testString.AsSpan();

            for (int i = 0; i < 10; ++i) {
                bool.Parse(span.ToString());
            }
        }

        [Benchmark]
        public void BoolParseSpan()
        {
            // 파라미터로 span 객체가 넘어 왔다고 가정 한다
            var span = testString.AsSpan();

            for (int i = 0; i < 10; ++i) {
                BoolParser.Parse(span);
            }
        }

        [Benchmark]
        public void BoolParseSpanInternal()
        {
            // 파라미터로 span 객체가 넘어 왔다고 가정 한다
            var span = testString.AsSpan();

            for (int i = 0; i < 10; ++i) {
                bool.Parse(span);
            }
        }
    }

    [MemoryDiagnoser]
    public class IntParseBenchMark
    {
        private string testString = int.MaxValue.ToString();

        [Benchmark]
        public void IntParse()
        {
            // 파라미터로 span 객체가 넘어 왔다고 가정 한다
            var span = testString.AsSpan();

            for (int i = 0; i < 10; ++i) {
                int.Parse(span.ToString());
            }
        }

        [Benchmark]
        public void IntSpanParse()
        {
            // 파라미터로 span 객체가 넘어 왔다고 가정 한다
            var span = testString.AsSpan();

            for (int i = 0; i < 10; ++i) {
                IntParser.Parse(in span);
            }
        }

        [Benchmark]
        public void IntSpanParseInternal()
        {
            // 파라미터로 span 객체가 넘어 왔다고 가정 한다
            var span = testString.AsSpan();

            for (int i = 0; i < 10; ++i) {
                int.Parse(span);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(BoolParseBenchMark).Assembly).Run(args);
        }
    }
}
