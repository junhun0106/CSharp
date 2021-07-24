using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication2
{
    public interface ICommandHandler
    {
        string Command { get; }
        string Description { get; }
        void Execute(string[] args);
    }

    public class TestCommandHandler : ICommandHandler
    {
        public string Command { get; } = "test";
        public string Description { get; } = "test description";

        public void Execute(string[] _)
        {
            Console.WriteLine($"{Command} : {Description}");
        }
    }

    public static class CommandExtensions
    {
        public static string[] Tokenize(this string s)
        {
            var cuts = new List<KeyValuePair<int, int>>();
            int i = 0;
            while (i < s.Length) {
                if (char.IsWhiteSpace(s[i])) {
                    ++i;
                } else if (s[i] == '\"') {
                    cuts.Add(ReadPhrase(s, ref i));
                } else {
                    cuts.Add(ReadWord(s, ref i));
                }
            }
            return cuts.Select(x => s.Substring(x.Key, x.Value)).Select(x => x.Replace("\\\"", "\"")).ToArray();

        }

        private static KeyValuePair<int, int> ReadPhrase(string s, ref int i)
        {
            ++i;
            int beg = i;
            int count = 0;
            while (i < s.Length) {
                if (s[i] == '\"' && (s[i - 1] != '\\'))
                    break;
                ++count;
                ++i;
            }
            ++i;
            ++i;
            return new KeyValuePair<int, int>(beg, count);
        }

        private static KeyValuePair<int, int> ReadWord(string s, ref int i)
        {
            int beg = i;
            int count = 1;
            ++i;
            while (i < s.Length) {
                if (char.IsWhiteSpace(s[i]))
                    break;
                ++count;
                ++i;
            }
            return new KeyValuePair<int, int>(beg, count);
        }
    }

    /// <summary>
    /// Debug 모드에서 프로그램 실행 중에 커맨드를 입력 가능하도록 한다
    /// </summary>
    public class InputHostedService : IHostedService
    {
        readonly Dictionary<string, ICommandHandler> _commandHandler;
        volatile bool _running;

        public InputHostedService()
        {
            _commandHandler = new Dictionary<string, ICommandHandler>(StringComparer.OrdinalIgnoreCase) {
                ["test"] = new TestCommandHandler(),
            };
        }

        public void ReadCommand()
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _running = true;
            // 윈도우 서비스 아닌 경우 커맨드 입력받기
            var task = Task.Factory.StartNew(() => {
                while (_running) {
                    while (_running) {
                        var line = Console.ReadLine();
                        if (line != null) {
                            var token = line.Tokenize();
                            if (token.Length > 0) {
                                var cmd = token[0];
                                if (_commandHandler.TryGetValue(cmd, out var command)) {
                                    command.Execute(token);
                                }
                            }
                        }
                    }
                    Console.Write("read cmdline terminated...");
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _running = false;

            return Task.CompletedTask;
        }
    }
}
