using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using ClassLibrary1;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;

namespace Tester
{
    [MemoryDiagnoser]
    public class DictionryLookUp
    {
        private readonly Dictionary<string, string> _dic = new Dictionary<string, string>(StringComparer.Ordinal) {
            ["a"] = "a",
            ["b"] = "b",
            ["c"] = "c",
            ["d"] = "d",
            ["e"] = "e",
            ["f"] = "f",
        };

        [Benchmark]
        public void ContainsKeyIndex()
        {
            if (_dic.ContainsKey("a")) {
                var _ = _dic["a"];
            }
        }

        [Benchmark]
        public void TryGetValue()
        {
            if (_dic.TryGetValue("a", out var _)) {

            }
        }

    }

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
            if (test is IDungeonMap) {

            }
        }

        [Benchmark]
        public void TryGet()
        {
            if (TryGetA(test, out var b)) {

            }
        }

        private bool TryGetA(IFieldMap a, out IDungeonMap b)
        {
            if(a is IDungeonMap dm) {
                b = dm;
                return true;
            }

            b = null;
            return false;
        }
    }

    [MemoryDiagnoser]
    public class EnumerableFirstOrDefault
    {
        private readonly Container container = new Container();

        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = container.FirstOrDefault(x => x.Value == "a");
        }

        [Benchmark]
        public void PredicateForeach()
        {
            var _ = container.GetOrDefault(x => x.Value == "a");
        }
    }

    [MemoryDiagnoser]
    public class ArrayFindBenchmark
    {
        private readonly Input[] _list = new Input[] {
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
        };

        [Benchmark]
        public void ArrayFind()
        {
            var _ = _list.Find(x => x.Value == "a");
        }

        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = _list.FirstOrDefault(x => x.Value == "a");
        }
    }

    [MemoryDiagnoser]
    public class ArrayFindAllBenchmark
    {
        private readonly Input[] _list = new Input[] {
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
            new Input("b"),
        };

        [Benchmark]
        public void ArrayFindAll()
        {
            var a = _list.FindAll(x => x.Value == "a");
        }

        [Benchmark]
        public void Where()
        {
            var a = _list.Where(x => x.Value == "a");
        }
    }

    [MemoryDiagnoser]
    public class XArrayContainsBenchmark
    {
        private Input[] _list;

        private Input _input;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _input = new Input("c");
            _list = new Input[] {
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("a"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                new Input("b"),
                _input,
            };
        }

        [Benchmark]
        public void BinarySearch()
        {
            if (_list.Exists_3(_input)) {
                //throw new Exception();
            } else {
                throw new Exception();
            }
        }

        [Benchmark]
        public void Foreach()
        {
            if (_list.Exists(_input)) {
                //throw new Exception();
            } else {
                throw new Exception();
            }
        }

        [Benchmark]
        public void For()
        {
            if (_list.Exists_2(_input)) {
                //throw new Exception();
            } else {
                throw new Exception();
            }
        }

        //[Benchmark]
        //public void ForStringOrdinalComparer()
        //{
        //    if (_list.Exists_2(_input)) {
        //        throw new Exception();
        //    }
        //}

        [Benchmark]
        public void ContainsComparerNull()
        {
            if (_list.Contains(_input, comparer: null)) {
                //throw new Exception();
            } else {
                throw new Exception();
            }
        }

        //[Benchmark]
        //public void ContainsComparerStringOrdinal()
        //{
        //    if (_list.Contains(_input, StringComparer.Ordinal)) {
        //        throw new Exception();
        //    }
        //}
    }

    [MemoryDiagnoser]
    public class DictionaryWhereBenchmark
    {
        private readonly Dictionary<string, Input> _list = new Dictionary<string, Input>(StringComparer.Ordinal) {
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
        };

        [Benchmark]
        public void Foreach()
        {
            foreach (var kv in _list) {
                if (kv.Value.Value == "a") {
                }
            }
        }

        [Benchmark]
        public void Where()
        {
            var wheres = _list.Where(kv => kv.Value.Value == "a");
            foreach (var item in wheres) {

            }
        }
    }

    [MemoryDiagnoser]
    public class DictionaryElementAt
    {
        private readonly Dictionary<string, Input> _list = new Dictionary<string, Input>(StringComparer.Ordinal) {
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["a"] = new Input("a"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
            ["b"] = new Input("b"),
        };

        [Benchmark]
        public void ElementAt()
        {
            var _ = _list.ElementAt(0);
        }

        [Benchmark]
        public void ElementAtOrDefault()
        {
            var _ = _list.ElementAtOrDefault(0);
        }

        [Benchmark]
        public void First()
        {
            var _ = _list.First();
        }

        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = _list.FirstOrDefault();
        }

        [Benchmark]
        public void Foreach()
        {
            var _ = _list.GetFirstOrDefault();
        }
    }

    public static class DicionaryExtensions
    {
        public static KeyValuePair<TKey, TValue> GetFirstOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
        {
            foreach (var kv in source) {
                return kv;
            }

            return default(KeyValuePair<TKey, TValue>);
        }
    }

    public static class ArrayExntensions
    {
        public static T Find<T>(this T[] array, Predicate<T> pred)
        {
            return Array.Find(array, pred);
        }

        public static T[] FindAll<T>(this T[] array, Predicate<T> match)
        {
            return Array.FindAll(array, match);
        }

        public static int BinarySearch<T>(this T[] array, T value)
        {
            return Array.BinarySearch(array, value);
        }

        public static bool Exists<T> (this T[] array, T value)
        {
                foreach (var item in array) {
                    if (item.Equals(value)) {
                        return true;
                    }
                }
            return false;
        }

        public static bool Exists_2<T>(this T[] array, T value, IEqualityComparer<T> comparer = null)
        {
            if (comparer == null) {
                for (int i = 0; i < array.Length; ++i) {
                    var item = array[i];
                    if (item.Equals(value)) {
                        return true;
                    }
                }
            } else {
                for (int i = 0; i < array.Length; ++i) {
                    var item = array[i];
                    if (comparer.Equals(item, value)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool Exists_3<T>(this T[] array, T value)
        {
            return Array.BinarySearch(array, value) > 0;
        }
    }

    [MemoryDiagnoser]
    public class ListFindBenchmark
    {
        private readonly List<string> _list = new List<string> {
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
        };

        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = _list.FirstOrDefault(x => x == "a");
        }

        [Benchmark]
        public void Find()
        {
            var _ = _list.Find(x => x == "a");
        }
    }

    [MemoryDiagnoser]
    public class ListLast
    {
        private readonly List<string> _list = new List<string> {
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
        };

        [Benchmark]
        public void LastOrDefault()
        {
            var _ = _list.LastOrDefault();
        }

        [Benchmark]
        public void Index()
        {
            if (_list.Count > 0) {
                var _ = _list[_list.Count - 1];
            }
        }
    }

    [MemoryDiagnoser]
    public class ListIndex0
    {
        private readonly List<string> _list = new List<string> {
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
        };

        [Benchmark]
        public void FirstOrDefault()
        {
            var _ = _list.FirstOrDefault();
        }

        [Benchmark]
        public void Index0()
        {
            if (_list.Count > 0) {
                var _ = _list[0];
            }
        }
    }

    [MemoryDiagnoser]
    public class LinqContains
    {
        private readonly string[] _list = new string[] {
            "a",
            "a",
            "a",
            "a",
            "a",
            "a",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
            "b",
        };

        [Benchmark]
        public void Contains_Linq()
        {
            if (_list.Contains("a")) {

            }
        }

        [Benchmark]
        public void Contains_LinqStringComparer()
        {
            if (_list.Contains("a", StringComparer.Ordinal)) {

            }
        }

        [Benchmark]
        public void Contains_Foreach()
        {
            if (_list.CustomContains("a")) {

            }
        }
    }

    [MemoryDiagnoser]
    public class EnumerableRange
    {
        [Benchmark]
        public void Range()
        {
            foreach (var idx in Enumerable.Range(1, 12)) {

            }
        }

        [Benchmark]
        public void For()
        {
            for (int idx = 1; idx <= 12; ++idx) {

            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkSwitcher.FromAssembly(typeof(SelectToListBenchmark).Assembly).Run(args);

            var customConfig = ManualConfig
              .Create(DefaultConfig.Instance)
              .AddValidator(JitOptimizationsValidator.FailOnError)
              .AddDiagnoser(MemoryDiagnoser.Default)
              .AddColumn(StatisticColumn.AllStatistics)
              .AddJob(Job.Default.WithRuntime(CoreRuntime.Core31))
              .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
              .AddExporter(DefaultExporters.Markdown);

            //BenchmarkRunner.Run<MySqlConnectorBenchmark>(customConfig);
            BenchmarkRunner.Run<ListFindBenchmark>(customConfig);
        }
    }
}
