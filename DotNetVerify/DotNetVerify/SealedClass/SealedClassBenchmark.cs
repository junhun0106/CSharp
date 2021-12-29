using System.Reflection;

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
}
