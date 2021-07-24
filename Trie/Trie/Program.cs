using System;
using System.Diagnostics;

namespace TextPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            const string msg = "일이삼사오육칠팔구십" + // 10
                               "일이삼사오육칠팔구십" + // 20
                               "일이삼사오육칠팔구십" + // 30
                               "일이삼사오육칠팔구십" + // 40
                               "일이삼사오육칠팔구십";  // 50

            Console.WriteLine($"필터 count : {TextFilter.Count}");

            const double count = 10000;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; ++i) {
                TextFilter.Filter(msg);
            }
            sw.Stop();

            Console.WriteLine("origin");
            Console.WriteLine($"\tavg:{sw.ElapsedMilliseconds / count}ms");

            sw.Restart();
            for (int i = 0; i < count; ++i) {
                TextFilter.Filter_Trie(msg);
            }
            sw.Stop();

            Console.WriteLine("trie");
            Console.WriteLine($"\tavg:{sw.ElapsedMilliseconds / count}ms");


            const string msg2 = "욕999";
            sw.Restart();
            for (int i = 0; i < count; ++i) {
                TextFilter.Filter(msg2);
            }
            sw.Stop();
            Console.WriteLine("origin-slang");
            Console.WriteLine($"\tavg:{sw.ElapsedMilliseconds / count}ms");
            sw.Restart();
            for (int i = 0; i < count; ++i) {
                TextFilter.Filter_Trie(msg2);
            }
            sw.Stop();

            Console.WriteLine("trie-slang");
            Console.WriteLine($"\tavg:{sw.ElapsedMilliseconds / count}ms");
            Console.ReadLine();
        }
    }
}
