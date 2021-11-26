using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using LinqBenchmark;

namespace LinqBenchmark.Benchmark
{
    public class WhereSelectBenchmark
    {
        private class A
        {
            public readonly string @string;

            public A(string @string)
            {
                this.@string = @string;
            }
        }

        private const int _capacity = 1000;
        private readonly List<A> _list = new List<A>(_capacity);

        [GlobalSetup]
        public void GlobalSetUp()
        {
            for (int i = 0; i < _capacity; ++i)
            {
                _list.Add(new A((i % 10).ToString()));
            }
        }

        [Benchmark]
        public void WhereSelectLinq()
        {
            _ = _list.Where(x => x.@string == "0").Select(x => x.@string);
        }

        [Benchmark]
        public void WhereSelectCustom()
        {
            _ = _list.WhereSelect(x => x.@string == "0", x => x.@string);
        }
    }
}
