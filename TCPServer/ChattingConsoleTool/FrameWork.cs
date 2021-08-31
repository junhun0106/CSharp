using ChattingMultiTool.Utilities;
using ClientLib.Chat.Clients;
using ClientLib.Chat.Structures;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ChattingMultiTool
{
    public class FrameWork
    {
        private const string _logger = nameof(FrameWork);

        private Task _mainTask;
        private CancellationTokenSource _cts;
        private readonly ParallelOptions _parallelOptions = new ParallelOptions {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        public void Begin(Config config)
        {
            Log.Information("begin process");
            _cts = new CancellationTokenSource();
            _mainTask = Task.Factory.StartNew(async () => {
                Log.Information("task begin");
                var clients = await ConnectAsync(config).ConfigureAwait(false);
                await UpdateAsync(config, clients).ConfigureAwait(false);

                foreach (var kv in clients) {
                    var client = kv.Value;
                    client.EndNetwork();
                }

                Log.Information("task end");
            }, _cts.Token);
        }

        public void End()
        {
            _cts.Cancel();
            try {
                _mainTask.Wait();
            } catch (OperationCanceledException) {
                // ignore
            } catch (Exception e) {
                Log.Error($"{_logger} : {e}");
            }

            _cts = null;
            _mainTask = null;

            lock (_totalLatency) {
                _totalLatency.Reset();
            }

            lock (_latencyData) {
                _latencyData.Clear();
            }

            Log.Information("end process");
        }

        private async Task DelayAsync(int ms = 1000)
        {
            try {
                if (_cts != null) {
                    await Task.Delay(ms, _cts.Token).ConfigureAwait(false);
                }
            } catch (TaskCanceledException) {
                // ignore
            }
        }

        private async Task<Dictionary<string, Client>> ConnectAsync(Config config)
        {
            var clientCount = config.ClientCount;
            var enterPerSec = config.EnterPerSec;
            var clients = new Dictionary<string, Client>(clientCount, StringComparer.Ordinal);
            var clientsLock = new object();
            var connectSuccessClients = new List<string>(clientCount);
            var connectFailClients = new List<string>(clientCount);
            // connect
            int tryCount = 0;
            while (clientCount > 0) {
                int count = 0;
                if (clientCount > enterPerSec) {
                    count = enterPerSec;
                } else {
                    count = clientCount;
                }
                tryCount += count;
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < count; ++i) {
                    var familyName = CreateName.Create();
                    //Log.Debug($"{_logger} : [{familyName} client connect try");
                    var client = new Client(familyName, config.Ip, config.Port) {
                        EnteredResponse = (msg) => {
                            if (!string.IsNullOrEmpty(msg)) {
                                Log.Error($"{_logger} : [{familyName}] client connect fail - {msg}");
                                lock (clientsLock) {
                                    connectFailClients.Add(familyName);
                                }
                            } else {
                                lock (clientsLock) {
                                    connectSuccessClients.Add(familyName);
                                }
                            }
                        },
                        ExtraLogging = (level, msg) => {
                            switch (level) {
                                case Client.ELogLevel.Debug:
                                    Log.Debug($"[{familyName}] {msg}");
                                    break;
                                case Client.ELogLevel.Warn:
                                    Log.Warning($"[{familyName}] {msg}");
                                    break;
                                case Client.ELogLevel.Error:
                                    Log.Error($"[{familyName}] {msg}");
                                    break;
                                case Client.ELogLevel.Fatal:
                                    Log.Fatal($"[{familyName}] {msg}");
                                    break;
                            }
                        },
                        ReceiveChatEvent = ReceiveEvent,
                    };
                    clients[familyName] = client;

                    // connection start
                    clients[familyName].Reconnect();
                }
                clientCount -= enterPerSec;
                
                foreach (var client in clients) {
                    client.Value.PostUpdate();
                }

                sw.Stop();
                Log.Debug($"{_logger} : {tryCount}/{config.ClientCount} connect try, et : {sw.ElapsedMilliseconds}ms");
                await DelayAsync().ConfigureAwait(false);
            }

            // wait connect
            int waitCount = 0;
            const int waitMS = 1000;
            while (true) {
                lock (clientsLock) {
                    foreach (var client in clients) {
                        client.Value.PostUpdate();
                    }

                    if (waitCount > 4) {
                        Log.Information($"{_logger} : long wait, {waitMS * waitCount}ms");
                        Log.Information($"{_logger} : client connect fail : {connectFailClients.Count}");
                        foreach (var fail in connectFailClients) {
                            if (clients.TryGetValue(fail, out var client)) {
                                client.EndNetwork();
                                clients.Remove(fail);
                            }
                        }
                        var longWaitClients = new List<string>();
                        foreach (var kv in clients) {
                            var familyName = kv.Key;
                            if (!connectSuccessClients.Contains(familyName)) {
                                longWaitClients.Add(familyName);
                            }
                        }
                        foreach (var longWait in longWaitClients) {
                            Log.Information($"[{longWait}] : client connect long wait, forced disconnect");
                            if (clients.TryGetValue(longWait, out var client)) {
                                client.EndNetwork();
                                clients.Remove(longWait);
                            }
                        }
                        longWaitClients.Clear();
                        break;
                    } else {
                        var totalCount = connectFailClients.Count + connectSuccessClients.Count;
                        if (totalCount == clients.Count) {
                            Log.Information($"{_logger} : client connect fail : {connectFailClients.Count}");
                            foreach (var fail in connectFailClients) {
                                if (clients.TryGetValue(fail, out var client)) {
                                    client.EndNetwork();
                                    clients.Remove(fail);
                                }
                            }
                            break;
                        }
                    }
                }

                await DelayAsync(waitMS).ConfigureAwait(false);

                waitCount++;

                Log.Information("{logger} : waitting cllient connect, waitCount:{waitCount}", _logger, waitCount);
            }

            Log.Information($"{_logger} : cllient connect : {clients.Count}");

            return clients;
        }

        private async Task UpdateAsync(Config config, Dictionary<string, Client> clients)
        {
            var now = DateTime.Now;
            var removeClients = new TSList<string>(clients.Count);
            var sw = Stopwatch.StartNew();
            sw.Stop();
            sw.Reset();
            var scenarioSw = Stopwatch.StartNew();
            scenarioSw.Stop();
            scenarioSw.Reset();
            var maxEt = 0L;
            var maxScenarioEt = 0L;
            while (_cts != null && !_cts.IsCancellationRequested) {
                if (clients.Count == 0) {
                    break;
                }

                var tickGuid = Guid.NewGuid().ToString();

                sw.Start();
                // update
                Parallel.ForEach(clients, _parallelOptions, kv => {
                    var client = kv.Value;
                    if (removeClients.Contains(client.Id)) {
                        return;
                    }

                    if (client.IsConnected) {
                        client.PostUpdate();
                        client.Update();
                    } else {
                        removeClients.Add(client.Id);
                    }
                });

                if (removeClients.Count > 0) {
                    foreach (var familyName in removeClients) {
                        clients.Remove(familyName);
                        Log.Debug($"[{familyName}] disconnect");
                    }
                    removeClients.Clear();
                }

                var deltaTime = (DateTime.Now - now).TotalSeconds;
                if (deltaTime >= 1.0d) {
                    scenarioSw.Start();
                    now = DateTime.Now;

                    var scenario = (Scenario.EScenario)config.ScenarioNum;
                    Scenario.UpdateScenario(scenario, this, config, clients, removeClients, _parallelOptions);
                    scenarioSw.Stop();
                    var scenarioEt = scenarioSw.ElapsedMilliseconds;
                    if (maxScenarioEt < scenarioEt) {
                        maxScenarioEt = scenarioEt;
                        Log.Debug($"[{tickGuid}] scenario update frame max et : {scenarioEt}ms");
                    }
                    scenarioSw.Reset();
                }
                sw.Stop();
                var et = sw.ElapsedMilliseconds;
                if (maxEt < et) {
                    maxEt = et;
                    Log.Debug($"[{tickGuid}] total update frame max et : {et}ms");
                }
                sw.Reset();
                await DelayAsync(0).ConfigureAwait(false);
            }
        }

        private class TotalLatency
        {
            public int count;
            public int failedCount;
            public TimeSpan timeMin = TimeSpan.MaxValue;
            public TimeSpan timeMax = TimeSpan.MinValue;
            public TimeSpan timeAvg = TimeSpan.Zero;

            public void Reset()
            {
                count = 0;
                failedCount = 0;
                timeMin = TimeSpan.MaxValue;
                timeMax = TimeSpan.MinValue;
                timeAvg = TimeSpan.Zero;
            }

            public override string ToString()
            {
                return $"count:{count}, failedCount:{failedCount}, min:{timeMin.TotalSeconds}sec, max{timeMax.TotalSeconds}sec, avg:{timeAvg.TotalSeconds}sec";
            }
        }

        private class LatencyData
        {
            public readonly string Guid;
            public readonly string FamilyName;
            public readonly string Action;
            public readonly DateTime Start;

            public LatencyData(string action, string guid, string familyName)
            {
                Action = action;
                Guid = guid;
                FamilyName = familyName;
                Start = DateTime.Now;
            }
        }

        private readonly TotalLatency _totalLatency = new TotalLatency();
        private readonly Dictionary<string, LatencyData> _latencyData = new Dictionary<string, LatencyData>(StringComparer.Ordinal);

        public void ShowLatency()
        {
            string msg;
            lock (_totalLatency) {
                msg = _totalLatency.ToString();
            }

            Log.Information(msg);
        }

        public void SendEvent(string action, string guid, string familyName)
        {
            //lock (_latencyData) {
            //    _latencyData[guid] = new LatencyData(action, guid, familyName);
            //}
        }

        private void ReceiveEvent(ChatMessage message)
        {
            Console.WriteLine($"[{message.SenderName}] : {message.Message}");
            //if (string.IsNullOrEmpty(message.Guid)) {
            //    return;
            //}

            //LatencyData latency = null;
            //lock (_latencyData) {
            //    if (_latencyData.TryGetValue(message.Guid, out latency)) {
            //        _latencyData.Remove(message.Guid);
            //    }
            //}

            //if (latency != null && latency.FamilyName == message.SenderName) {
            //    var et = DateTime.Now - latency.Start;
            //    lock (_totalLatency) {
            //        if (_totalLatency.timeMin > et) _totalLatency.timeMin = et;
            //        if (_totalLatency.timeMax < et) _totalLatency.timeMax = et;
            //        if (_totalLatency.timeAvg == TimeSpan.Zero) {
            //            _totalLatency.timeAvg = et;
            //        } else {
            //            _totalLatency.timeAvg = GetAvg(_totalLatency.timeAvg, et, _totalLatency.count);
            //        }
            //        _totalLatency.count++;
            //        if (message.Failed) {
            //            _totalLatency.failedCount++;
            //        }
            //    }
            //}
        }

        private TimeSpan GetAvg(TimeSpan prevAvg, TimeSpan x, int n)
        {
            return new TimeSpan(GetAvg(prevAvg.Ticks, x.Ticks, n));
        }

        private long GetAvg(long prevAvg, long x, int n)
        {
            return ((prevAvg * n) + x) / (n + 1);
        }
    }
}
