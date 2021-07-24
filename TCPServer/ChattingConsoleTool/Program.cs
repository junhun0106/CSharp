using System;
using System.Collections.Generic;
using Serilog;
using System.Globalization;
using ChattingMultiTool.Utilities;

namespace ChattingMultiTool
{
    internal sealed class Program
    {
        private static Config CreateConfig()
        {
            // todo : config file 읽어서 해야 함
            return new Config();
        }

        private static void CreateLogger(Config config)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.Async(configure => {
                //    configure.Console();
                //    configure.File(config.LogPath);
                //})
                .CreateLogger();
        }

        private static void Main(string[] _)
        {
            var config = CreateConfig();
            CreateLogger(config);

            CreateName.Init();

            var exits = new List<string>(2) { "e", "exit" };
            while (true) {
                var framework = new FrameWork();
                framework.Begin(config);
                while (true) {
                    Console.WriteLine($"if you want end of Multibot, press [{string.Join(",", exits)}]");
                    var line = Console.ReadLine();
                    if (exits.Contains(line.ToLower(CultureInfo.InvariantCulture))) {
                        break;
                    } else if (string.Equals(line, "l", StringComparison.OrdinalIgnoreCase)) {
                        framework.ShowLatency();
                    }
                }
                framework.End();

                // note : 프로그램 재시작이 아닌 로직에서 재시작 할 수 있도록
                Console.WriteLine($"if you want end of program, press [{string.Join(",", exits)}]");
                Console.WriteLine($"or want restart program, press any key excepting [{string.Join(",", exits)}]");
                var line2 = Console.ReadLine();
                if (exits.Contains(line2.ToLower(CultureInfo.InvariantCulture))) {
                    break;
                }
            }
        }
    }
}
