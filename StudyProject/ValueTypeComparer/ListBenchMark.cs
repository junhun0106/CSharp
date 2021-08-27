using System;
using System.Collections.Generic;
using System.Text;

using BenchmarkDotNet.Attributes;

namespace ValueTypeComparer
{
    public class TestA
    {
        public int A;
        public int B;
        public long C;
        public ulong D;

        public bool Find(TestA x)
        {
            return x.A == A && x.B == B && x.C == C && x.D == D;
        }
    }

    public struct StructComparer : IComparer<TestA>, IEqualityComparer<TestA>
    {
        public int Compare(TestA x, TestA y)
        {
            if (x.A == y.A && x.B == y.B && x.C == y.C && x.D == y.D) {
                return 0;
            }

            return 1;
        }

        public bool Equals(TestA x, TestA y)
        {
            return x.A == y.A && x.B == y.B && x.C == y.C && x.D == y.D;
        }

        public int GetHashCode(TestA obj)
        {
            return obj.A.GetHashCode() ^ obj.B.GetHashCode() ^ obj.C.GetHashCode() ^ obj.D.GetHashCode();
        }
    }

    [MemoryDiagnoser]
    public class ListBenchMark
    {
        TestA a;

        List<TestA> a_list;

        StructComparer comparer;

        [GlobalSetup]
        public void Init()
        {
            a = new TestA();

            a_list = new List<TestA>();
            a_list.Add(new TestA { A = 1, B = 1, C = 1, D = 1 });
            a_list.Add(new TestA { A = 1, B = 1, C = 1, D = 1 });
            a_list.Add(new TestA { A = 2, B = 1, C = 1, D = 1 });
            a_list.Add(new TestA { A = 2, B = 1, C = 1, D = 1 });
            a_list.Add(new TestA { A = 2, B = 1, C = 1, D = 1 });
            a_list.Add(new TestA { A = 2, B = 1, C = 1, D = 1 });

            comparer = new StructComparer();
        }

        [Benchmark]
        public void CallDefault()
        {
            for (int i = 0; i < 10; ++i) {
                a_list.Contains(a);
            }
        }

        [Benchmark]
        public void CallBinarySearch()
        {
            for (int i = 0; i < 10; ++i) {
                a_list.BinarySearch(a, comparer);
            }
        }

        [Benchmark]
        public void CallFind_Lambda()
        {
            for (int i = 0; i < 10; ++i) {
                a_list.Find(x => x.A == a.A && x.B == a.B && x.C == a.C && x.D == a.D);
            }
        }

        [Benchmark]
        public void CallFind_LocalFunc()
        {
            for (int i = 0; i < 10; ++i) {
                a_list.Find(find);
            }

            bool find(TestA x) => x.A == a.A && x.B == a.B && x.C == a.C && x.D == a.D;
        }

        [Benchmark]
        public void CallFind_MemberFunc()
        {
            for (int i = 0; i < 10; ++i) {
                for (int j = 0; j < a_list.Count; ++j) {
                    var x = a_list[j];
                    if (a.Find(x)) {
                        // nothing
                    }
                }
            }
        }
    }
}
