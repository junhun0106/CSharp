using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;

namespace ValueTypeComparer
{
    public static class EnumComparer
    {
        public static TestComparer TestComparer = new TestComparer();
    }

    public static class EnumComparer<T> where T : Enum
    {
        /// <summary>
        /// https://referencesource.microsoft.com/#mscorlib/system/collections/generic/equalitycomparer.cs,d8e28972e89a3e86
        /// </summary>
        private struct DefaultEnumComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : Enum
        {
            private readonly IEqualityComparer<TEnum> comparer;

            public DefaultEnumComparer(IEqualityComparer<TEnum> comparer)
            {
                this.comparer = comparer;
            }

            public bool Equals(TEnum x, TEnum y) => comparer.Equals(x, y);

            public int GetHashCode(TEnum obj) => comparer.GetHashCode(obj);
        }

        public static IEqualityComparer<T> Default { get; } = new DefaultEnumComparer<T>(EqualityComparer<T>.Default);
    }

    public class TestComparer : EqualityComparer<ETest>
    {
        public override bool Equals(ETest x, ETest y)
        {
            return (int)x == (int)y;
        }

        public override int GetHashCode(ETest obj)
        {
            return (int)obj;
        }
    }

    public enum ETest
    {
        Test0,
        Test1,
        Test2,
        Test3,
    }

    [MemoryDiagnoser]
    public class EnumBenchMark
    {
        Dictionary<ETest, ETest> dic1;
        //Dictionary<ETest, ETest> dic2;
        Dictionary<ETest, ETest> dic3;

        [GlobalSetup]
        public void Init()
        {
            dic3 = new Dictionary<ETest, ETest>(EnumComparer.TestComparer) {
                { ETest.Test0, ETest.Test0 },
                { ETest.Test1, ETest.Test1 },
                { ETest.Test2, ETest.Test2 },
                { ETest.Test3, ETest.Test3 }
            };

            dic1 = new Dictionary<ETest, ETest> {
                { ETest.Test0, ETest.Test0 },
                { ETest.Test1, ETest.Test1 },
                { ETest.Test2, ETest.Test2 },
                { ETest.Test3, ETest.Test3 }
            };

            //dic2 = new Dictionary<ETest, ETest>(EnumComparer<ETest>.Default);
            //dic2.Add(ETest.Test0, ETest.Test0);
            //dic2.Add(ETest.Test1, ETest.Test1);
            //dic2.Add(ETest.Test2, ETest.Test2);
            //dic2.Add(ETest.Test3, ETest.Test3);
        }

        [Benchmark]
        public void CallNormal()
        {
            for (int i = 0; i < 10; ++i) {
                dic3.ContainsKey(ETest.Test0);
                dic3.ContainsKey(ETest.Test1);
                dic3.ContainsKey(ETest.Test2);
                dic3.ContainsKey(ETest.Test3);
            }
        }

        [Benchmark]
        public void CallDefault()
        {
            for (int i = 0; i < 10; ++i) {
                dic1.ContainsKey(ETest.Test0);
                dic1.ContainsKey(ETest.Test1);
                dic1.ContainsKey(ETest.Test2);
                dic1.ContainsKey(ETest.Test3);
            }
        }

        //[Benchmark]
        //public void CallCustom()
        //{
        //    for (int i = 0; i < 10; ++i) {
        //        dic2.ContainsKey(ETest.Test0);
        //        dic2.ContainsKey(ETest.Test1);
        //        dic2.ContainsKey(ETest.Test2);
        //        dic2.ContainsKey(ETest.Test3);
        //    }
        //}
    }
}
