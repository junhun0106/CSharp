using System.Text;
using System.Collections.Generic;
using ClassLibrary1;
using System;

namespace MemoryProfile
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<int>(10000);
            for (int i = 0; i < 10000; ++i) {
                list.Add(i);
            }

            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var i in list) {
                if (sb.Length == 0) {
                    sb.Append(i.ToString());
                } else {
                    sb.Append($"; {i}");
                }
            }
            sb.Append("}");

            var testString = sb.ToString();
            for (int i = 1; i <= 1000; ++i) {
                testString.ToStringArraySafe();
                testString.AsSpan().ToStringArraySafe();
                testString.AsSpan().ToStringArraySafe_2();
                testString.AsSpan().ToStringArraySafe_Custom();

                if (i % 100 == 0) {
                    Console.WriteLine($"{i} test...");
                }
            }
        }
    }
}
