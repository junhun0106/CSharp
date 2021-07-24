using System;
using BenchmarkDotNet.Attributes;
using Util;

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
        private string toStringArray_property => "{v1_longclassname, v2_longclassname; v3_longclassname}";

        [Benchmark]
        public void ToStringArray_String()
        {
            var strings = toStringArray_property.ToStringArraySafe();
            if (strings.Length != 3) throw new Exception();
            if (strings[0] != "v1_longclassname") throw new Exception();
            if (strings[1] != "v2_longclassname") throw new Exception();
            if (strings[2] != "v3_longclassname") throw new Exception();
        }

        [Benchmark]
        public void ToStringArray_Span()
        {
            var strings = toStringArray_property.AsSpan().ToStringArraySafe();
            if (strings.Length != 3) throw new Exception();
            if (strings[0] != "v1_longclassname") throw new Exception();
            if (strings[1] != "v2_longclassname") throw new Exception();
            if (strings[2] != "v3_longclassname") throw new Exception();
        }

        [Benchmark]
        public void ToStringArray_Span2()
        {
            var strings = toStringArray_property.AsSpan().ToStringArraySafe_2();
            if (strings.Length != 3) throw new Exception();
            if (strings[0] != "v1_longclassname") throw new Exception();
            if (strings[1] != "v2_longclassname") throw new Exception();
            if (strings[2] != "v3_longclassname") throw new Exception();
        }

        [Benchmark]
        public void ToStringArray_Span3()
        {
            var strings = toStringArray_property.AsSpan().ToStringArraySafe_3();
            if (strings.Length != 3) throw new Exception();
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
}
