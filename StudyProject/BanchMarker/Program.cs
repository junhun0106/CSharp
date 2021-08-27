using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using ValueTypeComparer;

namespace BanchMarker
{
    public class DirectAssignVsPropertySetBenchMark
    {
        public class AAA
        {
            public string Name { get; set; }
        }

        public class BBB
        {
            public string Name;
        }

        public class CCC
        {
            public readonly string Name;

            public CCC(string name)
            {
                Name = name;
            }
        }

        public class DDD
        {
            public string Name { get; }

            public DDD(string name)
            {
                Name = name;
            }
        }

        [Benchmark]
        public void PropertySet()
        {
            var aaa = new AAA();
            aaa.Name = "AAA";
        }

        [Benchmark]
        public void PropertyInit()
        {
            var aaa = new DDD("AAA");
        }

        [Benchmark]
        public void DirectAssign()
        {
            var aaa = new BBB {
                Name = "AAA",
            };
        }

        [Benchmark]
        public void CtorMemberVal()
        {
            var aaa = new CCC("AAA");
        }
    }

    public class AbstarctVsInterfaceBenchMark
    {
        public abstract class AAA_1
        {
            public abstract string Call();
        }

        public class AAA_2
        {
            public virtual string Call()
            {
                var logger = nameof(AAA_2);
                return $"{logger} call";
            }
        }

        public interface IAAA
        {
            string Call();
        }

        public class BBB : AAA_1
        {
            public override string Call()
            {
                var logger = nameof(BBB);
                return $"{logger} child from {nameof(AAA_1)}";
            }
        }

        public class CCC : AAA_2
        {
            public override string Call()
            {
                var logger = nameof(CCC);
                return $"{logger} child from {nameof(AAA_2)}";
            }
        }

        public class DDD : IAAA
        {
            public string Call()
            {
                var logger = nameof(DDD);
                return $"{logger} child from {nameof(IAAA)}";
            }
        }

        public class Manager
        {
            public string Call(AAA_1 obj)
            {
                return $"{obj.Call()} in manager";
            }

            public string Call(AAA_2 obj)
            {
                return $"{obj.Call()} in manager";
            }

            public string Call(IAAA obj)
            {
                return $"{obj.Call()} in manager";
            }
        }

        private AAA_1 bbb;
        private AAA_2 ccc;
        private IAAA ddd;

        private Manager manager;

        [GlobalSetup]
        public void Init()
        {
            bbb = new BBB();
            ccc = new CCC();
            ddd = new DDD();

            manager = new Manager();
        }

        [Benchmark]
        public void CallAbstract()
        {
            bbb.Call();
            bbb.Call();
            bbb.Call();
        }

        [Benchmark]
        public void CallAbstractCasting()
        {
            manager.Call(bbb);
            manager.Call(bbb);
            manager.Call(bbb);
        }

        [Benchmark]
        public void CallVirtual()
        {
            ccc.Call();
            ccc.Call();
            ccc.Call();
        }

        [Benchmark]
        public void CallVirtualCasting()
        {
            manager.Call(ccc);
            manager.Call(ccc);
            manager.Call(ccc);
        }

        [Benchmark]
        public void CallInterface()
        {
            ddd.Call();
            ddd.Call();
            ddd.Call();
        }

        [Benchmark]
        public void CallInterfaceCasting()
        {
            manager.Call(ddd);
            manager.Call(ddd);
            manager.Call(ddd);
        }
    }

    [MemoryDiagnoser]
    public class TestBenchMark
    {
        public struct TestA
        {
            public double timestamp;
            public float x;
            public float y;
            public bool moveType;
        }

        public class TestB
        {
            public double timestamp;
            public float x;
            public float y;
            public bool moveType;
        }

        TestA a;
        TestB b;

        [GlobalSetup]
        public void Init()
        {
            a = new TestA();
            b = new TestB();
        }

        [Benchmark]
        public void A()
        {
            for (int i = 0; i < 10; ++i) {
                a.moveType = false;
                a.timestamp = i;
                a.x = i;
                a.y = i;
            }
        }

        [Benchmark]
        public void B()
        {
            for (int i = 0; i < 10; ++i) {
                b.moveType = false;
                b.timestamp = i;
                b.x = i;
                b.y = i;
            }
        }
    }

    [MemoryDiagnoser]
    public class StringBenchMark
    {
        static char[] separator = new char[] { '@' };
        string testString = "123@str";

        [Benchmark]
        public void Split()
        {
            var split = testString.Split(separator);
            var s1 = split[0];
        }

        [Benchmark]
        public void Span()
        {
            var span = testString.AsSpan();
            var index = span.IndexOfAny(separator);
            var s1 = span.Slice(0, index).ToString();
        }

        [Benchmark]
        public void SubString()
        {
            var index = testString.IndexOfAny(separator);
            var s1 = testString.Substring(0, index);
        }
    }

    [MemoryDiagnoser]
    public class IntParseBenchMark
    {
        const string parseString = "1234";

        [Benchmark]
        public void ConvertToInt32()
        {
            for (int i = 0; i < 100; ++i) {
                Convert.ToInt32(parseString);
            }
        }

        [Benchmark]
        public void ConvertToInt32FromBase()
        {
            for (int i = 0; i < 100; ++i) {
                Convert.ToInt32(parseString, 10);
            }
        }

        [Benchmark]
        public void IntParse()
        {
            for (int i = 0; i < 100; ++i) {
                int.Parse(parseString);
            }
        }

        [Benchmark]
        public void TryIntParse()
        {
            for (int i = 0; i < 100; ++i) {
                int.TryParse(parseString, out var _);
            }
        }
    }

    [MemoryDiagnoser]
    public class GetStringBenchMark
    {
        byte[] testBytes;

        [GlobalSetup]
        public void Init()
        {
            const string s = "abcdefghijklmnopqrstuvwxyz";

            testBytes = System.Text.Encoding.UTF8.GetBytes(s);
        }

        [Benchmark]
        public void EncodingUTF8GetString()
        {
            for (int i = 0; i < 10; ++i) {
                var _ = System.Text.Encoding.UTF8.GetString(testBytes);
            }
        }

        [Benchmark]
        public void EncodingUTF8GetStringSpan()
        {
            for (int i = 0; i < 10; ++i) {
                var _ = System.Text.Encoding.UTF8.GetString(testBytes.AsSpan());
            }
        }
    }

    class Program
    {
        class TestClass
        {
            public string a1 = "a1";
            public string a2 = "a2";
            public string a3 = "a3";
            public string a4 = "a4";
            public string a5 = "a5";
        }

        struct TestStruct
        {
            public TestClass t1;
            public TestClass t2;
            public TestClass t3;
            public TestClass t4;
            public TestClass t5;

            public string a1;
            public string a2;
            public string a3;
            public string a4;
            public string a5;

            public TestStruct(int c = 0)
            {
                a1 = "a11";
                a2 = "a22";
                a3 = "a33";
                a4 = "a44";
                a5 = "a55";

                t1 = new TestClass();
                t2 = new TestClass();
                t3 = new TestClass();
                t4 = new TestClass();
                t5 = new TestClass();
            }
        }

        private static void Call()
        {
            var variable = new TestStruct();
            //variable.a1 = "a111";
            //variable.a2 = "a222";
            //variable.a3 = "a333";
            //variable.a4 = "a444";
            //variable.a5 = "a555";

            //variable.t1.a1 = "a11111";
            //variable.t1.a2 = "a11111";
            //variable.t1.a3 = "a11111";
            //variable.t1.a4 = "a11111";
            //variable.t1.a5 = "a11111";
        }

        [Flags]
        enum SpanSplitInfo : byte
        {
            RemoveEmptyEntries = 0x1,
            FinishedEnumeration = 0x2,
        }

        private static void threadFunc()
        {
            SpanSplitInfo flag = SpanSplitInfo.FinishedEnumeration;
            while (true) {
                flag |= SpanSplitInfo.RemoveEmptyEntries;
            }
        }

        static void Main(string[] args)
        {
            const string s = "abcdefghijkl,mnopqrstuvwxyz";

            //byte[] @byte = new byte[] { 44 };
            //var testBytes = System.Text.Encoding.UTF8.GetBytes(s);
            //var span = testBytes.AsSpan();
            //var index = span.IndexOfAny(@byte);
            //var slice = span.Slice(0, index);
            //var slice2 = span.Slice(index + 1);
            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(slice));
            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(slice2));
            //Console.WriteLine((byte)'\n');
            Thread t = new Thread(threadFunc);
            t.IsBackground = true;
            t.Start();

            while (true) {
                int n = GC.CollectionCount(0) + GC.CollectionCount(1) + GC.CollectionCount(2);
                Console.WriteLine(n);

                Thread.Sleep(50);
            }

            //var dic = new Dictionary<ETest, ETest>(EnumComparer<ETest>.Default);
            //dic.Add(ETest.Test0, ETest.Test0);
            //dic.Add(ETest.Test1, ETest.Test1);
            //dic.Add(ETest.Test2, ETest.Test2);
            //dic.Add(ETest.Test3, ETest.Test3);

            //Console.WriteLine(dic.ContainsKey(ETest.Test0));
            //Console.WriteLine(dic.ContainsKey(ETest.Test1));
            //Console.WriteLine(dic.ContainsKey(ETest.Test2));
            //Console.WriteLine(dic.ContainsKey(ETest.Test3));

            //var list = new List<TestA>();
            //list.Add(new TestA { A = 1, B = 1, C = 1, D = 1 });
            //list.Add(new TestA { A = 1, B = 1, C = 1, D = 1 });
            //list.Add(new TestA { A = 2, B = 1, C = 1, D = 1 });
            //list.Add(new TestA { A = 2, B = 1, C = 1, D = 1 });
            //list.Add(new TestA { A = 2, B = 1, C = 1, D = 1 });
            //list.Add(new TestA { A = 2, B = 1, C = 1, D = 1 });

            //var comparer = new TestA { A = 1, B = 1, C = 1, D = 1 };

            //Console.WriteLine(list.Contains(comparer));
            //Console.WriteLine(list.BinarySearch(comparer, new StructComparer()));

            //var dic = new Dictionary<TestA, int>();
            //dic.Add(new TestA { A = 1, B = 1, C = 1, D = 1 }, 1);
            //dic.Add(new TestA { A = 2, B = 1, C = 1, D = 1 }, 2);
            //dic.Add(new TestA { A = 3, B = 1, C = 1, D = 1 }, 3);
            //dic.Add(new TestA { A = 4, B = 1, C = 1, D = 1 }, 4);
            //dic.Add(new TestA { A = 5, B = 1, C = 1, D = 1 }, 5);
            //Console.WriteLine(dic.ContainsKey(comparer));

#if DEBUG
            //BenchmarkSwitcher.FromAssembly(typeof(BanchMarker.AbstarctVsInterfaceBenchMark).Assembly).Run(args, new DebugInProcessConfig());
#elif RELEASE
            BenchmarkSwitcher.FromAssembly(typeof(TestBenchMark).Assembly).Run(args);
#endif
        }
    }
}
