using System;
using System.Text;
using System.Collections.Generic;

namespace TwoQueueMakeOneStack
{
    internal static class Program
    {
        private static void Main(string[] _)
        {
            var sb = new StringBuilder();

            var list = new List<int> { 1, 2, 3, 4, 5 };
            var queue = new Queue<int>();
            var stack = new Stack<int>();

            foreach (var l in list) {
                queue.Enqueue(l);
                stack.Push(l);
            }

            sb.AppendLine("original queue");
            foreach (var q in queue) {
                sb.AppendLine($"\t{q}");
            }

            sb.AppendLine("original stack");
            foreach (var s in stack) {
                sb.AppendLine($"\t{s}");
            }

            var qs = new StackQueue<int>(list.Count);
            foreach (var l in list) {
                qs.Push(l);
            }

            sb.AppendLine("2 queue make 1 stack");
            while (!qs.IsEmpty) {
                var qsValue = qs.Pop();
                var stackValue = stack.Pop();
                sb.AppendLine($"\tqs : {qsValue} == stack:{stackValue} = {qsValue == stackValue}");
            }

            Console.WriteLine(sb.ToString());
            Console.WriteLine("complete test... press any key...");
            Console.ReadLine();
        }
    }
}
