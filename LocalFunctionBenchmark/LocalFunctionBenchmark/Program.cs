using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace LocalFunctionBenchmark
{
    [MemoryDiagnoser()]
    public class DeletgateBenchmark
    {
        private struct A
        {
            public readonly int Index;
            public readonly string @string;

            public A(int index, string @string)
            {
                Index = index;
                this.@string = @string;
            }
        }

        private const int _capacity = 1000;
        private readonly List<A> _list = new(_capacity);

        [GlobalSetup]
        public void GlobalSetUp()
        {
            for (int i = 0; i < _capacity; ++i)
            {
                var index = (i % 10);
                _list.Add(new A(index, index.ToString()));
            }
        }

        [Benchmark]
        public void Func()
        {
            var _comparer = new A(0, "0");
            _ = _list.Where(x => x.Index == _comparer.Index).Select(x => new A(_comparer.Index, _comparer.Index.ToString()));
        }

        [Benchmark]
        public void LocalFunc()
        {
            var _comparer = new A(0, "0");
            bool where(A x) => x.Index == _comparer.Index;
            A select(A x) => new A(_comparer.Index, _comparer.Index.ToString());
            _ = _list.Where(x => where(x)).Select(x => select(x));
        }

        // 파라미터를 넣은 경우가 더 느린 것 확인하여 더 이상 테스트 할 필요 없어서 주석
        //[Benchmark]
        //public void LocalFuncInParameter()
        //{
        //    bool where(A x) => x.@string == "0";
        //    string select(A x) => x.@string; 
        //    _ = _list.Where(where).Select(select);
        //}

        // static로 변환이 가능한 func이라면, 모두가 같은 할당을 보인다
        // 즉, static로 선언한 경우는 힙 할당을 피하겠다는 의미.
        // 더 이상 테스트 할 필요가 없으므로 제외.
        //[Benchmark]
        //public void StaticLocalFunc()
        //{
        //    static bool where(A x) => x.@string == "0"; 
        //    static string select(A x) => x.@string;
        //    _ = _list.Where(x => where(x)).Select(x => select(x));
        //}

        // 파라미터를 넣은 경우가 더 느린 것 확인하여 더 이상 테스트 할 필요 없어서 주석
        //[Benchmark]
        //public void StaticLocalFuncInParameter()
        //{
        //    static bool where(A x) => x.@string == "0";
        //    static string select(A x) => x.@string;
        //    _ = _list.Where(where).Select(select);
        //}

        // static 함수와 static local 함수가 다른 점이 없음 확을 확인하여 주석
        //[Benchmark]
        //public void StaticFunc()
        //{
        //    _ = _list.Where(x => g_where(x)).Select(x => g_select(x));
        //}

        // 파라미터를 넣은 경우가 더 느린 것 확인하여 더 이상 테스트 할 필요 없어서 주석
        //[Benchmark]
        //public void StaticFuncInParameter()
        //{
        //    _ = _list.Where(g_where).Select(g_select);
        //}

        //static bool g_where(A x) => x.@string == "0";
        //static string g_select(A x) => x.@string;
    }

    internal static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(DeletgateBenchmark).Assembly).Run(args);
            
            //var customConfig = ManualConfig
            //    .Create(DefaultConfig.Instance)
            //    .AddValidator(JitOptimizationsValidator.FailOnError)
            //    .AddDiagnoser(MemoryDiagnoser.Default)
            //    .AddColumn(StatisticColumn.AllStatistics)
            //    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
            //    .AddExporter(DefaultExporters.Markdown);

            //BenchmarkRunner.Run<DeletgateBenchmark>(customConfig);
        }
    }
}
