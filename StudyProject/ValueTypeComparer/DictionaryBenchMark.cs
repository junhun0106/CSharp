using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;

namespace ValueTypeComparer
{
    [MemoryDiagnoser]
    public class DictionaryBenchMark
    {
        Dictionary<TestA, int> dic1;
        Dictionary<TestA, int> dic2;

        TestA compare;

        [GlobalSetup]
        public void Init()
        {
            compare = new TestA();
            dic1 = new Dictionary<TestA, int>();
            dic1.Add(new TestA { A = 1, B = 1, C = 1, D = 1 }, 1);
            dic1.Add(new TestA { A = 2, B = 1, C = 1, D = 1 }, 2);
            dic1.Add(new TestA { A = 3, B = 1, C = 1, D = 1 }, 3);
            dic1.Add(new TestA { A = 4, B = 1, C = 1, D = 1 }, 4);
            dic1.Add(new TestA { A = 5, B = 1, C = 1, D = 1 }, 5);
            dic1.Add(new TestA(), 6);

            dic2 = new Dictionary<TestA, int>(new StructComparer());
            dic2.Add(new TestA { A = 1, B = 1, C = 1, D = 1 }, 1);
            dic2.Add(new TestA { A = 2, B = 1, C = 1, D = 1 }, 2);
            dic2.Add(new TestA { A = 3, B = 1, C = 1, D = 1 }, 3);
            dic2.Add(new TestA { A = 4, B = 1, C = 1, D = 1 }, 4);
            dic2.Add(new TestA { A = 5, B = 1, C = 1, D = 1 }, 5);
            dic2.Add(new TestA(), 6);
        }

        [Benchmark]
        public void CallDefault()
        {
            for (int i = 0; i < 10; ++i) dic1.ContainsKey(compare);
        }

        [Benchmark]
        public void CallComparer()
        {
            for (int i = 0; i < 10; ++i) dic2.ContainsKey(compare);
        }
    }
}
