using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using IntParserLib;

namespace IntParse
{
    [MemoryDiagnoser]
    public class IntParseBenchMark
    {
        private static readonly string testString = int.MaxValue.ToString();

        [Benchmark]
        public void IntParse()
        {
            for (int i = 0; i < 10; ++i) {
                int.Parse(testString);
            }
        }

        //[Benchmark]
        //public void IntTryParse()
        //{
        //    for (int i = 0; i < 10; ++i) {
        //        int.TryParse(testString, out var _);
        //    }
        //}

        //[Benchmark]
        //public void IntParse_Span()
        //{
        //    for (int i = 0; i < 10; ++i) {
        //        var span = testString.AsSpan();
        //        int.Parse(span);
        //    }
        //}

        //[Benchmark]
        //public void IntTryParse_Span()
        //{
        //    for (int i = 0; i < 10; ++i) {
        //        var span = testString.AsSpan();
        //        int.TryParse(span, out var _);
        //    }
        //}

        [Benchmark]
        public void IntPrase_ASCII()
        {
            for (int j = 0; j < 10; ++j) {
                IntParser.Parse(in testString);
            }
        }
    }

    [MemoryDiagnoser]
    public class BoolParseBenchMark
    {
        private const string testString = "true";

        [Benchmark]
        public void BoolParse()
        {
            for (int i = 0; i < 10; ++i) {
                bool.Parse(testString);
            }
        }

        [Benchmark]
        public void BoolParse_Internal()
        {
            // note : 내부 코드에서 필요한 부분만 가져와서 처리
            // https://referencesource.microsoft.com/#mscorlib/system/boolean.cs,8b58385ae061c937
            for (int i = 0; i < 10; ++i) {
                var result = false;
                // "True"가 아니면 모두 false로 처리 한다
                var s = testString.Trim();
                if ("True".Equals(s, StringComparison.OrdinalIgnoreCase)) {
                    result = true;
                }
            }
        }

        [Benchmark]
        public void BoolParse_Custom()
        {
            for (int i = 0; i < 10; ++i) {
                bool b = false;
                if (testString.Length > 0) {
                    var s = testString.Trim();
                    var s0 = s[0];
                    b = s0 == 't' || s0 == 'T'; // string[0] 값이 t 또는 T가 아니면 false
                }
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
