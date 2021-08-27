using IntParserLib;
using System;

namespace MemoryProfile
{
    class Tester
    {
        public void Test_1(in string s)
        {
            for (int i = 0; i < 100000; ++i) {
                int.TryParse(s, out var _);
            }

            for (int i = 0; i < 1000; ++i) {
                var _ = i.ToString();
            }
        }


        public void Test_3(in ReadOnlySpan<char> span)
        {
            for(int i =0; i < 100000; ++i)
                IntParser.Parse(in span);

            for (int i = 0; i < 1000; ++i) {
                var _ = i.ToString();
            }
        }
    }

    internal static class Program
    {
        private static void Main(string[] _)
        {
            var s = int.MaxValue.ToString(provider: null);
            var span = s.AsSpan();
            const int testCount = 1000;
            for (int i = 1; i <= testCount; ++i) {
                Tester tester = new Tester();
                tester.Test_1(in s);
                tester.Test_3(in span);
                if (i % 100 == 0) {
                    Console.WriteLine($"{i} test...");
                }
            }

            Console.WriteLine("complete...");
        }
    }
}
