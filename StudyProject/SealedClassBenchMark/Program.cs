using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace SealedClassBenchMark
{
    public class SealedClassJustCallBenchMark
    {
        public class AAA
        {
            public string Call()
            {
                var logger = nameof(AAA);
                return $"{logger} call {nameof(Call)}";
            }
        }

        public sealed class BBB
        {
            public string Call()
            {
                var logger = nameof(BBB);
                return $"{logger} call {nameof(Call)}";
            }
        }

        private AAA aaa;
        private BBB bbb;

        [GlobalSetup]
        public void Init()
        {
            aaa = new AAA();
            bbb = new BBB();
        }

        [Benchmark]
        public void CallRegular()
        {
            aaa.Call();
            aaa.Call();
            aaa.Call();
        }

        [Benchmark]
        public void CallSealed()
        {
            bbb.Call();
            bbb.Call();
            bbb.Call();
        }
    }

    public class SealedClassToStringBenchMark
    {
        public class AAA
        {
            public override string ToString()
            {
                var logger = nameof(AAA);
                return $"{logger} call {nameof(ToString)}";
            }
        }

        public sealed class BBB
        {
            public override string ToString()
            {
                var logger = nameof(BBB);
                return $"{logger} call {nameof(ToString)}";
            }
        }

        private AAA aaa;
        private BBB bbb;

        [GlobalSetup]
        public void Init()
        {
            aaa = new AAA();
            bbb = new BBB();
        }

        [Benchmark]
        public void CallRegular()
        {
            aaa.ToString();
            aaa.ToString();
            aaa.ToString();
        }

        [Benchmark]
        public void CallSealed()
        {
            bbb.ToString();
            bbb.ToString();
            bbb.ToString();
        }
    }

    public class SealedClassInheritanceBenchMark
    {
        public class AAA
        {
            public virtual string Call()
            {
                return nameof(AAA);
            }
        }

        public class BBB : AAA
        {
            public override string Call()
            {
                var logger = nameof(BBB);
                return $"{logger} child from {nameof(AAA)}";
            }
        }

        public sealed class CCC : AAA
        {
            public override string Call()
            {
                var logger = nameof(CCC);
                return $"{logger} child from {nameof(AAA)}";
            }
        }

        private BBB bbb;
        private CCC ccc;

        [GlobalSetup]
        public void Init()
        {
            bbb = new BBB();
            ccc = new CCC();
        }

        [Benchmark]
        public void CallRegular()
        {
            bbb.Call();
            bbb.Call();
            bbb.Call();
        }

        [Benchmark]
        public void CallSealed()
        {
            ccc.Call();
            ccc.Call();
            ccc.Call();
        }
    }

    public class SealedClassInterfaceBenchMark
    {
        interface IInterface
        {
            string Call();
        }

        public class AAA : IInterface
        {
            public string Call()
            {
                var logger = nameof(AAA);
                return $"{logger} call {nameof(Call)}";
            }
        }

        public sealed class BBB : IInterface
        {
            public string Call()
            {
                var logger = nameof(BBB);
                return $"{logger} call {nameof(Call)}";
            }
        }

        private IInterface aaa;
        private IInterface bbb;

        [GlobalSetup]
        public void Init()
        {
            aaa = new AAA();
            bbb = new BBB();
        }

        [Benchmark]
        public void CallRegular()
        {
            aaa.Call();
            aaa.Call();
            aaa.Call();
        }

        [Benchmark]
        public void CallSealed()
        {
            bbb.Call();
            bbb.Call();
            bbb.Call();
        }
    }

    public class SealedMemberInheritanceBenchMark
    {
        public class AAA
        {
            public virtual string Call()
            {
                return nameof(AAA);
            }
        }

        public class BBB : AAA
        {
            public override string Call()
            {
                var logger = nameof(BBB);
                return $"{logger} child from {nameof(AAA)}";
            }
        }

        public class CCC : AAA
        {
            public sealed override string Call()
            {
                var logger = nameof(CCC);
                return $"{logger} child from {nameof(AAA)}";
            }
        }

        private BBB bbb;
        private CCC ccc;

        [GlobalSetup]
        public void Init()
        {
            bbb = new BBB();
            ccc = new CCC();
        }

        [Benchmark]
        public void CallRegular()
        {
            bbb.Call();
            bbb.Call();
            bbb.Call();
        }

        [Benchmark]
        public void CallSealed()
        {
            ccc.Call();
            ccc.Call();
            ccc.Call();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            BenchmarkSwitcher.FromAssembly(typeof(SealedClassBenchMark.SealedClassJustCallBenchMark).Assembly).Run(args, new DebugInProcessConfig());
#elif RELEASE
            BenchmarkSwitcher.FromAssembly(typeof(SealedClassBenchMark.SealedClassJustCallBenchMark).Assembly).Run(args);
#endif
        }
    }
}
