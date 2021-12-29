namespace DotNetVerify.SealedClass
{
    [MemoryDiagnoser]
    public class SealedAttributeBenchmark
    {
        class NoneSealedAttribute : Attribute
        {
        }

        sealed class SealedAttribute : Attribute
        {
        }

        [NoneSealed]
        class CNoneSealedAttribute { }


        [Sealed]
        class CSealedAttribute { }

        private readonly CNoneSealedAttribute _noneSealed = new CNoneSealedAttribute();
        private readonly CSealedAttribute _sealed = new CSealedAttribute();

        [Benchmark]
        public void NoneSealed()
        {
            _ = _noneSealed.GetType().GetCustomAttribute<NoneSealedAttribute>();
        }

        [Benchmark]
        public void Sealed()
        {
            _ = _sealed.GetType().GetCustomAttribute<SealedAttribute>();
        }
    }

    [MemoryDiagnoser]
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

    [MemoryDiagnoser]
    public class SealedClassInheritanceBenchMark
    {
        public class BaseType
        {
            public virtual string Call() => nameof(BaseType);
        }

        public class BBB : BaseType
        {
            public override string Call()
            {
                return $"{nameof(BBB)} child from {nameof(BaseType)}";
            }
        }

        public class CCC : BaseType
        {
            public sealed override string Call()
            {
                return $"{nameof(CCC)} child from {nameof(BaseType)}";
            }
        }


        private readonly BBB _bbb = new ();
        private readonly CCC _ccc = new ();

        [Benchmark]
        public void NonSealed()
        {
            _bbb.Call();
            _bbb.Call();
            _bbb.Call();
        }

        [Benchmark]
        public void Sealed()
        {
            _ccc.Call();
            _ccc.Call();
            _ccc.Call();
        }
    }

    [MemoryDiagnoser]
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

    [MemoryDiagnoser]
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

    [MemoryDiagnoser]
    public class SealedClassPerformanceImprovement
    {
        private SealedType _sealed = new();
        private NonSealedType _nonSealed = new();

        [Benchmark(Baseline = true)]
        public int NonSealed() => _nonSealed.M() + 42;

        [Benchmark]
        public int Sealed() => _sealed.M() + 42;

        public class BaseType
        {
            public virtual int M() => 1;
        }

        public class NonSealedType : BaseType
        {
            public override int M() => 2;
        }

        public sealed class SealedType : BaseType
        {
            public override int M() => 2;
        }
    }
}
