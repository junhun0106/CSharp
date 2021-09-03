using BenchmarkDotNet.Running;

namespace MemoryPoolBenchMarker
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(MemoryPoolRentBenchMark).Assembly).Run(args);
        }
    }
}
