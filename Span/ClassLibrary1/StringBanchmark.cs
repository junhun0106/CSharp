using System;
using BenchmarkDotNet.Attributes;

namespace ClassLibrary1
{
    [MemoryDiagnoser]
    public class ToStringIntDictionaryBenchmark
    {
        [Benchmark]
        public void ToStringIntDictionary_String()
        {
            var property = "{v1_longclassname:1, v2_longclassname:2; v3_longclassname:3}";
            var dic = property.ToStringIntDictionary();
            if (dic.Count != 3) throw new Exception();
            if (dic["v1_longclassname"] != 1) throw new Exception();
            if (dic["v2_longclassname"] != 2) throw new Exception();
            if (dic["v3_longclassname"] != 3) throw new Exception();
        }


        [Benchmark]
        public void ToStringIntDictionary_Span()
        {
            var property = "{v1_longclassname:1, v2_longclassname:2; v3_longclassname:3}";
            var dic = property.AsSpan().ToStringIntDictionary();
            if (dic.Count != 3) throw new Exception();
            if (dic["v1_longclassname"] != 1) throw new Exception();
            if (dic["v2_longclassname"] != 2) throw new Exception();
            if (dic["v3_longclassname"] != 3) throw new Exception();
        }
    }

    [MemoryDiagnoser]
    public class ToStringArrayBenchmark
    {
        private string toStringArray_property => "{v1_longclassname, v2_longclassname; v3_longclassname; v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname;v3_longclassname}";

        [Benchmark]
        public void ToStringArray_String()
        {
            var strings = toStringArray_property.ToStringArraySafe();
            if (strings[0] != "v1_longclassname") throw new Exception();
            if (strings[1] != "v2_longclassname") throw new Exception();
            if (strings[2] != "v3_longclassname") throw new Exception();
        }

        [Benchmark]
        public void ToStringArray_NotResize()
        {
            var strings = toStringArray_property.AsSpan().ToStringArraySafeNotResize();
            if (strings[0] != "v1_longclassname") throw new Exception();
            if (strings[1] != "v2_longclassname") throw new Exception();
            if (strings[2] != "v3_longclassname") throw new Exception();
        }

        [Benchmark]
        public void ToStringArray_SpanSplitEnumerator()
        {
            var strings = toStringArray_property.AsSpan().ToStringArraySafe();
            if (strings[0] != "v1_longclassname") throw new Exception();
            if (strings[1] != "v2_longclassname") throw new Exception();
            if (strings[2] != "v3_longclassname") throw new Exception();
        }

        [Benchmark]
        public void ToStringArray_SpanSplitEnumerator2()
        {
            var strings = toStringArray_property.AsSpan().ToStringArraySafe_2();
            if (strings[0] != "v1_longclassname") throw new Exception();
            if (strings[1] != "v2_longclassname") throw new Exception();
            if (strings[2] != "v3_longclassname") throw new Exception();
        }

        [Benchmark]
        public void ToStringArray_Span()
        {
            var strings = toStringArray_property.AsSpan().ToStringArraySafe_Custom();
            if (strings[0] != "v1_longclassname") throw new Exception();
            if (strings[1] != "v2_longclassname") throw new Exception();
            if (strings[2] != "v3_longclassname") throw new Exception();
        }
    }

    [MemoryDiagnoser]
    public class ToStringListDictionaryBenchmark
    {
        private string rawProperty => "{k1_classname:v1_longparam0, k2_longclassname:v2_longparam0, k1_classname:v3_param1}";

        [Benchmark]
        public void ToStringArray_String()
        {
            var strings = rawProperty.ToStringListDictionary();
            if (strings.Count != 2) throw new Exception();
            if (strings["k1_classname"].Count != 2) throw new Exception();
            if (strings["k1_classname"][0] != "v1_longparam0") throw new Exception();
            if (strings["k1_classname"][1] != "v3_param1") throw new Exception();

            if (strings["k2_longclassname"].Count != 1) throw new Exception();
            if (strings["k2_longclassname"][0] != "v2_longparam0") throw new Exception();
        }

        [Benchmark]
        public void ToStringArray_Span()
        {
            var strings = rawProperty.AsSpan().ToStringListDictionary();
            if (strings.Count != 2) throw new Exception();
            if (strings["k1_classname"].Count != 2) throw new Exception();
            if (strings["k1_classname"][0] != "v1_longparam0") throw new Exception();
            if (strings["k1_classname"][1] != "v3_param1") throw new Exception();

            if (strings["k2_longclassname"].Count != 1) throw new Exception();
            if (strings["k2_longclassname"][0] != "v2_longparam0") throw new Exception();
        }
    }

    [MemoryDiagnoser]
    public class ToStringStringBenchmark
    {
        private const string property = "long_test_string:long_test_string";

        [Benchmark]
        public void ToStringString_String()
        {
            var kv = property.ToStringString();
            if (kv.Key != "long_test_string") throw new Exception();
            if (kv.Value != "long_test_string") throw new Exception();
        }

        [Benchmark]
        public void ToStringString_Span()
        {
            var kv = property.AsSpan().ToStringString();
            if (kv.Key != "long_test_string") throw new Exception();
            if (kv.Value != "long_test_string") throw new Exception();
        }
    }

    [MemoryDiagnoser]
    public class ToStringDoublePairDictionaryBenchMark
    {
        private const string testString = "{1:1:1,2:1:1,3:1:1,4:1:1,5:1:1,6:1:1,7:1:1,8:1:1,9:1:1}";

        [Benchmark]
        public void ToStringDoublePairDictionary()
        {
            var _ = testString.ToStringDoublePairDictionary();
        }

        [Benchmark]
        public void ToStringDoublePairDictionaryWithSpan()
        {
            var _ = testString.AsSpan().ToStringDoublePairDictionary();
        }
    }
}
