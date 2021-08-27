using BenchmarkDotNet.Running;

namespace ConsoleApp25
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            //BenchmarkSwitcher.FromAssembly(typeof(ClassLibrary1.ToStringIntDictionaryBenchmark).Assembly).Run(args, new DebugInProcessConfig());
#elif RELEASE
            BenchmarkSwitcher.FromAssembly(typeof(ClassLibrary1.ToStringIntDictionaryBenchmark).Assembly).Run(args);
#endif
        }
}
}
