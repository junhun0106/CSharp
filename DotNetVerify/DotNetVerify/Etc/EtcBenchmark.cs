using System.Runtime.CompilerServices;

namespace DotNetVerify.Etc
{
    [MemoryDiagnoser]
    public class CastingBenchmark
    {
        class A
        {
            public void Call()
            {
                //
            }
        }

        public object a = new A();

        [Benchmark]
        public void Direct()
        {
            for (int i = 0; i < 1_000; ++i)
                ((A)a).Call();
        }

        [Benchmark]
        public void As()
        {
            for (int i = 0; i < 1_000; ++i)
                (a as A).Call();
        }
    }

    [MemoryDiagnoser]
    public class StringLowerBenchmark
    {
        public const string A = "TRUE";

        [Benchmark]
        public void Lower()
        {
            for (int i = 0; i < 1_000; ++i)
            {
                if (A.ToLower() == "true")
                {

                }
            }
        }

        [Benchmark]
        public void OrdinalIgnoreCase()
        {
            for (int i = 0; i < 1_000; ++i)
            {
                if (string.Equals(A, "true", StringComparison.OrdinalIgnoreCase))
                {

                }
            }
        }
    }

    [MemoryDiagnoser]
    public class ListFindBenchmark
    {
        public readonly List<int> _list = new List<int>
        {
            1,2,3,4,5,6,7,8,9, 10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,
        };

        [Benchmark]
        public void FirstOrDefault()
        {
            for (int i = 0; i < 1_000; ++i)
            {
                _ = _list.FirstOrDefault(x => x == i);
            }
        }

        [Benchmark]
        public void Find()
        {
            for (int i = 0; i < 1_000; ++i)
            {
                _ = _list.Find(x => x == i);
            }
        }
    }

    [MemoryDiagnoser]
    public class ListConvertAllBenchmark
    {
        public readonly List<int> _list = new List<int>
        {
            1,2,3,4,5,6,7,8,9, 10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,
        };

        [Benchmark]
        public void SelectToList()
        {
            for (int i = 0; i < 1_000; ++i)
            {
                _ = _list.Select(x => x).ToList();
            }
        }

        [Benchmark]
        public void ConvertAll()
        {
            for (int i = 0; i < 1_000; ++i)
            {
                _ = _list.ConvertAll(x => x);
            }
        }
    }

    public static class CA1822
    {
        public static int Member;
    }

    [MemoryDiagnoser]
    public class CA1822Benchmark
    {
        /*
         CA1822 멤버를 static으로 표시하세요
         https://docs.microsoft.com/ko-kr/dotnet/fundamentals/code-analysis/quality-rules/ca1822
        */

        private int _member;

        [Benchmark]
        public void Call()
        {
            for (int i = 0; i < 1_000; ++i)
                CallInternal();
        }

        [Benchmark]
        public void StaticCall()
        {
            for (int i = 0; i < 1_000; ++i)
                StaticCallInternal();
        }

        private void CallInternal()
        {
            _member += 2;
        }

        private static void StaticCallInternal()
        {
            CA1822.Member += 2;
        }
    }

    [MemoryDiagnoser]
    public class RCS1214Benchmark
    {
        // Unnecessary interpolated string

        [Benchmark(Baseline = true)]
        public void Normal()
        {
            for (int i = 0; i < 1_000; ++i)
                Log("log");
        }

        [Benchmark]
        public void UnnecessaryInterpolatedString()
        {
            for (int i = 0; i < 1_000; ++i)
                Log($"log");
        }


        private void Log(string message)
        {
            //
        }
    }

    [MemoryDiagnoser]
    public class RecordBenchmakr
    {
        record RecordA : IEquatable<StructB>
        {
            public int A;
            public string B;

            public bool Equals(StructB other) => A == other.A && B == other.B;
        }

        struct StructA
        {
            public int A;
            public string B;
        }

        struct StructB : IEquatable<StructB>
        {
            public int A;
            public string B;

            public bool Equals(StructB other) => A == other.A && B == other.B;
        }

        private readonly List<RecordA> _records = new List<RecordA> {
            new RecordA { A = 1, B = "1" },
            new RecordA { A = 2, B = "2" },
            new RecordA { A = 3, B = "3" },
            new RecordA { A = 4, B = "4" },
        };
        private readonly List<StructA> _structs = new List<StructA>
        {
            new StructA { A = 1, B = "1" },
            new StructA { A = 2, B = "2" },
            new StructA { A = 3, B = "3" },
            new StructA { A = 4, B = "4" },
        };

        private readonly List<StructB> _structBs = new List<StructB>
        {
            new StructB { A = 1, B = "1" },
            new StructB { A = 2, B = "2" },
            new StructB { A = 3, B = "3" },
            new StructB { A = 4, B = "4" },
        };

        [Benchmark]
        public void Record()
        {
            if (!_records.Contains(new RecordA { A = 1, B = "1" }))
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void Struct()
        {
            if (!_structs.Contains(new StructA { A = 1, B = "1" }))
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void StructOverrideEquals()
        {
            if (!_structBs.Contains(new StructB { A = 1, B = "1" }))
            {
                throw new Exception();
            }
        }
    }
}
