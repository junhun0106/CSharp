using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;

using Tester.Benchmark;
using System.Runtime.CompilerServices;

namespace Tester
{
    [MemoryDiagnoser]
    public class IsBanchmark
    {
        private IFieldMap test = new A();

        public interface IFieldMap
        {

        }

        public interface IDungeonMap : IFieldMap
        {

        }

        public class A : IFieldMap, IDungeonMap
        {

        }

        [Benchmark]
        public void Is()
        {
            if (test is IDungeonMap)
            {

            }
        }

        [Benchmark]
        public void TryGet()
        {
            if (TryGetA(test, out var b))
            {

            }
        }

        private bool TryGetA(IFieldMap a, out IDungeonMap b)
        {
            if (a is IDungeonMap dm)
            {
                b = dm;
                return true;
            }

            b = null;
            return false;
        }
    }

    [MemoryDiagnoser]
    public class AggresiveInlineBenchmark
    {
        [Benchmark]
        public void Sum()
        {
            for (int i = 0; i < 100; ++i) Sum(i, i);
        }

        [Benchmark]
        public void SumInline()
        {
            for (int i = 0; i < 100; ++i) SumInline(i, i);
        }

        private static int Sum(int a, int b) => a + b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SumInline(int a, int b) => a + b;
    }

    internal static class Program
    {
        private static void Main(string[] args)
        {
            //BenchmarkSwitcher.FromAssembly(typeof(SelectToListBenchmark).Assembly).Run(args);

            var customConfig = ManualConfig
              .Create(DefaultConfig.Instance)
              .AddValidator(JitOptimizationsValidator.FailOnError)
              .AddDiagnoser(MemoryDiagnoser.Default)
              .AddColumn(StatisticColumn.AllStatistics)
              .AddJob(Job.Default.WithRuntime(ClrRuntime.Net48))
              .AddJob(Job.Default.WithRuntime(CoreRuntime.Core31))
              .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
              .AddExporter(DefaultExporters.Markdown);

            BenchmarkRunner.Run<AggresiveInlineBenchmark>(customConfig);
        }
    }
}
