using System.Runtime.InteropServices;

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
    public class SealedClassBenchmakr
    {
        private readonly NonSealedType _nonSealed = new NonSealedType();
        private readonly SealedType _sealed = new SealedType();
        private readonly BaseType _nonSealedBase = new NonSealedType();
        private readonly BaseType _sealedBase = new SealedType();


        [Benchmark(Baseline = true)]
        public int NonSealed() => _nonSealed.M() + 42;

        [Benchmark]
        public int Sealed() => _sealed.M() + 42;

        [Benchmark]
        public int NonSealedBase() => _nonSealedBase.M() + 42;


        [Benchmark]
        public int SealedBase() => _sealedBase.M() + 42;


        public abstract class BaseType
        {
            public abstract int M();
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

    [MemoryDiagnoser]
    public class StringInterpolateBechmark
    {
        private string _s1 = "hello";
        private string _s2 = "world";

        [Benchmark]
        public string Format() => string.Format("{0} {1}", _s1, _s2);
        [Benchmark]
        public string Interpolated() => $"{_s1} {_s2}";
    }
}
