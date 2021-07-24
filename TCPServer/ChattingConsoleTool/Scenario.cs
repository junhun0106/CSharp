using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces;
using ChattingMultiTool.Utilities;
using ClientLib.Chat.Clients;

namespace ChattingMultiTool
{
    using ScenarioAction = Action<Config, FrameWork, IReadOnlyDictionary<string, Client>, ICollection<string>, ParallelOptions>;

    public static class Scenario
    {
        public enum EScenario {
            Chatting = 1,
        }

        private static readonly Dictionary<EScenario, ScenarioAction> _scenario = new Dictionary<EScenario, ScenarioAction> {
            [EScenario.Chatting] = ChattingScenario,
        };

        public static bool UpdateScenario(EScenario scenario, FrameWork frameWork, Config config, IReadOnlyDictionary<string, Client> clients, ICollection<string> ignore, ParallelOptions parallelOptions)
        {
            if (_scenario.TryGetValue(scenario, out var action)) {
                action(config, frameWork, clients, ignore, parallelOptions);
                return true;
            }

            return false;
        }


        private static void ChattingScenario(Config config, FrameWork frameWork, IReadOnlyDictionary<string, Client> clients, ICollection<string> ignore, ParallelOptions parallelOptions)
        {
            Parallel.ForEach(clients, parallelOptions, kv => {
                var client = kv.Value;
                if (ignore.Contains(client.Id)) {
                    return;
                }

                if (client.IsConnected) {
                    for (int i = 0; i < config.SendPacketPerSec; ++i) {
                        var msg = CreateMessage.Create();
                        //var guid = Guid.NewGuid().ToString();
                        //frameWork.SendEvent("WorldChatting", guid, client.FamilyName);
                        client.ReqChat(msg);
                    }
                }
            });
        }
    }
}
