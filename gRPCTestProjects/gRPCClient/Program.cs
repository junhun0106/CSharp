using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading;

using gRPCLib.Proto;
using gRPCLib.Models;
using System.Linq;
using System.Threading.Tasks;
using gRPCLib.Convert;
using Grpc.Core;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace gRPCClient
{
    public class GrpcChannelData
    {
        public int Index;
        public ChannelBase Channel;
    }

    public class GrpcClient
    {
        public object @lock = new object();
        public bool failover;
        public int Handle;
        public int ChannelIndex;
        public gRPCService.gRPCServiceClient Client;

        public override string ToString() => $"ChannelIndex:{ChannelIndex}";
    }

    public class GrpcResponse
    {
        public int ChannelIndex;
        public int ClientHandle;
        public Exception InnerException;
    }

    class Program
    {
        private volatile static bool _running;
        private readonly static ConcurrentDictionary<int, GrpcChannelData> _channels = new ConcurrentDictionary<int, GrpcChannelData>();
        private readonly static ConcurrentDictionary<int, GrpcClient> _clients = new ConcurrentDictionary<int, GrpcClient>();
        private readonly static List<GrpcResponse> _responses = new List<GrpcResponse>();
        private readonly static ConcurrentDictionary<int, long> _elapsedTimes = new ConcurrentDictionary<int, long>();

        private static async Task Update(int index)
        {
            var request = new gRPCLib.Models.PingRequest();
            while (_running) {
                if (!_clients.TryGetValue(index, out var grpcClient)) {
                    break;
                }

                gRPCService.gRPCServiceClient client;
                bool failover = false;
                lock (grpcClient.@lock) {
                    failover = grpcClient.failover;
                    client = grpcClient.Client;
                }

                if (failover) {
                    await Task.Delay(500);
                    continue;
                }

                var result = new GrpcResponse();

                var sw = Stopwatch.StartNew();

                try {
                    var grpcResponse = await client.RpcPingRequestAsync(request.Convert());
                    var response = grpcResponse.Convert();
                } catch (ObjectDisposedException ode) when (ode.ObjectName == "SafeHandle") {
                    sw.Stop();
                    lock (grpcClient.@lock) {
                        _elapsedTimes[grpcClient.Handle] = sw.ElapsedMilliseconds;
                        result.ChannelIndex = grpcClient.ChannelIndex;
                        result.ClientHandle = grpcClient.Handle;
                        grpcClient.failover = true;
                    }
                    result.InnerException = ode;
                    Console.WriteLine($"{index} failover");
                } catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable) {
                    sw.Stop();
                    lock (grpcClient.@lock) {
                        _elapsedTimes[grpcClient.Handle] = sw.ElapsedMilliseconds;
                        result.ChannelIndex = grpcClient.ChannelIndex;
                        result.ClientHandle = grpcClient.Handle;
                        grpcClient.failover = true;
                    }
                    result.InnerException = e;
                    Console.WriteLine($"{index} failover");
                } catch (Exception e) {
                    Console.WriteLine($"{e.Message}");
                    await Task.Delay(1000);
                }

                lock (_responses) {
                    _responses.Add(result);
                }

                if (result.InnerException != null) {
                    await Task.Delay(100);
                } else {
                    await Task.Delay(500);
                }
            }
            Console.WriteLine($"{index} close");
        }

        private static async Task ResponseUpdate()
        {
            while (_running) {
                List<GrpcResponse> list = null;
                lock (_responses) {
                    if (_responses.Count == 0) {
                        continue;
                    }
                    list = new List<GrpcResponse>(_responses);
                    _responses.Clear();
                }

                var error = new Dictionary<int, List<GrpcResponse>>();

                foreach (var response in list) {
                    if (response.InnerException == null) {
                        continue;
                    }

                    var channelHandle = response.ChannelIndex;

                    if (error.TryGetValue(channelHandle, out var errorList)) {
                        errorList.Add(response);
                    } else {
                        errorList = new List<GrpcResponse> { response };
                        error.Add(channelHandle, errorList);
                    }
                }

                var index = 0;
                foreach (var errorClient in error) {
                    var channelIndex = errorClient.Key;
                    if (_channels.TryGetValue(channelIndex, out var channel)) {
                        channel.Channel.ShutdownAsync().Wait();
                        _channels.TryRemove(channelIndex, out var _);
                    }
                    if (_channels.Count > 0) {
                        var newChannel = _channels.ElementAt(index++ % _channels.Count).Value;
                        foreach (var errorResponse in errorClient.Value) {
                            if (_clients.TryGetValue(errorResponse.ClientHandle, out var grpcClient)) {
                                lock (grpcClient.@lock) {
                                    grpcClient.failover = false;
                                    grpcClient.ChannelIndex = newChannel.Index;
                                    grpcClient.Client = new gRPCService.gRPCServiceClient(newChannel.Channel);
                                }
                            }
                        }
                    } else {
                        foreach (var errorResponse in errorClient.Value) {
                            _clients.TryRemove(errorResponse.ClientHandle, out var _);
                        }
                    }
                }

                await Task.Delay(10);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("start");
            Console.ReadLine();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var opts = new List<ChannelOption>(2);
            opts.Add(new ChannelOption("grpc.keepalive_time_ms", 1000));
            opts.Add(new ChannelOption("grpc.keepalive_timeout_ms", 500));

            var coreChannels = new List<ChannelBase>(2);
            coreChannels.Add(new Channel($"localhost:{Ports.Port1}", ChannelCredentials.Insecure, opts));
            coreChannels.Add(new Channel($"localhost:{Ports.Port2}", ChannelCredentials.Insecure, opts));
            //coreChannels.Add(GrpcChannel.ForAddress($"http://localhost:{Ports.Port1}", new GrpcChannelOptions()));
            //coreChannels.Add(GrpcChannel.ForAddress($"http://localhost:{Ports.Port2}", new GrpcChannelOptions()));

            var random = new Random();

            for (int i = 0; i < 2; ++i) {
                var data1 = new GrpcChannelData {
                    Index = i,
                    Channel = coreChannels[i % coreChannels.Count],
                };
                _channels.TryAdd(i, data1);
            }

            const int testClients = 100;

            _running = true;

            var dic = new Dictionary<int, int>(_channels.Count);
            for (int i = 0; i < testClients; ++i) {
                var clientIndex = i;
                var channel = _channels.ElementAt(i % _channels.Count).Value;
                _clients.TryAdd(clientIndex, new GrpcClient {
                    Handle = clientIndex,
                    Client = new gRPCService.gRPCServiceClient(channel.Channel),
                    ChannelIndex = channel.Index,
                });
                if (dic.ContainsKey(channel.Index)) {
                    dic[channel.Index]++;
                } else {
                    dic[channel.Index] = 1;
                }
                Task.Run(() => Update(clientIndex));
            }

            Task.Run(ResponseUpdate);

            while (true) {
                if (_running) {
                    var line = Console.ReadLine();
                    if (line == "e") {
                        _running = false;
                        break;
                    }
                }
            }

            Console.WriteLine("end");
            Console.ReadLine();
        }
    }
}
