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
using System.Collections;

namespace Tester
{
    public class Container : IEnumerable<Input>
    {
        private readonly IEnumerable<Input> _list = new List<Input> {
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

        public Input GetOrDefault(Predicate<Input> predicate)
        {
            foreach(var input in _list) {
                if (predicate(input)) {
                    return input;
                }
            }

            return null;
        }

        public IEnumerator<Input> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }

    public class Input
    {
        public string Value;

        public Input(string value)
        {
            Value = value;
        }
    }

    public class Output
    {
        public string Value;

        public Output(string value)
        {
            Value = value;
        }
    }

    [MemoryDiagnoser]
    public class KeyValueBenchmark
    {
        [Benchmark]
        public void ToStringString_KeyValuePair()
        {
            const string testString = "test:test";
            var kv = testString.ToStringString();
            if (kv.Key != "test") throw new Exception();
            if (kv.Value != "test") throw new Exception();
        }

        [Benchmark]
        public void ToStringString_KeyValuePairSpan()
        {
            const string testString = "test:test";
            var kv = testString.AsSpan().ToStringString_Span();
            if (kv.Key != "test") throw new Exception();
            if (kv.Value != "test") throw new Exception();
        }

        [Benchmark]
        public void ToStringString_Tuple()
        {
            const string testString = "test:test";
            var kv = testString.ToStringString_Tuple();
            if (kv.Item1 != "test") throw new Exception();
            if (kv.Item2 != "test") throw new Exception();
        }

        [Benchmark]
        public void ToStringString_TupleSpan()
        {
            const string testString = "test:test";
            var kv = testString.AsSpan().ToStringString_TupleSpan();
            if (kv.Item1 != "test") throw new Exception();
            if (kv.Item2 != "test") throw new Exception();
        }
    }

    [MemoryDiagnoser]
    public class ToStringBenchmark
    {
        const int a = 1234;
        object obj = a;

        [Benchmark]
        public void A()
        {
            if (obj is int @int) {
                var str = @int.ToString();
            }
        }

        [Benchmark]
        public void B()
        {
            if (obj is int @int) {
                var str = @int.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    [MemoryDiagnoser]
    public class JsonBenchmark
    {
        private class TestData
        {
            public string a1 = "1234";
            public string a2 = "1234";
            public string a3 = "1234";
            public string a4 = "1234";
            public string a5 = "1234";
            public string a6 = "1234";
            public string a7 = "1234";
            public string a8 = "1234";
            public string a9 = "1234";
        }

        private string _jsonText;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            var data = new TestData();
            _jsonText = JsonConvert.SerializeObject(data);
        }

        [Benchmark]
        public void NewtonSoftJson()
        {
            var _ = JsonConvert.DeserializeObject<TestData>(_jsonText);
        }

        [Benchmark]
        public void SystemTextJson()
        {

        }

        [Benchmark]
        public void Utf8Json()
        {

        }
    }

    [Serializable]
    public class TestData2
    {
        public string a1 = "1234";
        public string a2 = "1234";
        public string a3 = "1234";
        public string a4 = "1234";
        public string a5 = "1234";
        public string a6 = "1234";
        public string a7 = "1234";
        public string a8 = "1234";
        public string a9 = "1234";
    }

    [MemoryDiagnoser]
    public class SelectToListBenchmark
    {
        private readonly Input[] _list = new Input[] {
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
        };

        [Benchmark]
        public void SelectToList()
        {
            var _ = _list.Select(x => new Output(x.Value)).ToList();
        }

        [Benchmark]
        public void ConvertAll()
        {
            var _ = _list.ConvertAll(x => new Output(x.Value));
        }
    }

    [MemoryDiagnoser]
    public class WhereToListBenchmark
    {
        private readonly Input[] _list = new Input[] {
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
        };

        [Benchmark]
        public void WhererToList()
        {
            var _ = _list.Where(x => x.Value == "a").ToList();
        }

        [Benchmark]
        public void ToListPredicate()
        {
            var _ = _list.ToList(x => x.Value == "a");
        }
    }

    [MemoryDiagnoser]
    public class WhereSelectToListBenchmark
    {
        private readonly Input[] _list = new Input[] {
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
            new Input("a"),
        };

        [Benchmark]
        public void WhererSelectToList()
        {
            var _ = _list.Where(x => x.Value == "a").Select(x => new Output(x.Value)).ToList();
        }

        [Benchmark]
        public void ToListPredicateConvertAll()
        {
            var _ = _list.ToList(x => x.Value == "a").ConvertAll(x => new Output(x.Value));
        }
    }

    public static class EnumExtensions
    {
        public static List<TOutput> ConvertAll<TInput, TOutput>(this IReadOnlyList<TInput> source, Converter<TInput, TOutput> converter)
        {
            var list = new List<TOutput>(source.Count);
            foreach(var item in source) {
                list.Add(converter(item));
            }
            return list;
        }

        public static List<T> ToList<T>(this IReadOnlyList<T> source, Predicate<T> predicate)
        {
            if (source?.Count > 0) {
                var list = new List<T>(source.Count);
                foreach (var item in source) {
                    if (predicate(item)) {
                        list.Add(item);
                    }
                }
                return list;
            }

            return new List<T>();
        }

        public static bool CustomContains<T>(this IReadOnlyList<T> source, T item)
        {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            if (source != null && source?.Count > 0) {
                foreach (var t in source) {
                    if (t.Equals(item)) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
