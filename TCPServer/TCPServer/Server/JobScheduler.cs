using ChatService.Clients;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace ChatService
{
    public class JobScheduler
    {
        private readonly Server _server;

        private readonly BlockingCollection<IJob> _jobQueue;

        private readonly ConcurrentQueue<(DateTime regTime, IClient client)> _waitQueue;

        /// <summary>
        /// 채팅 서버의 메인 로직을 담당하는 쓰레드
        /// </summary>
        private readonly Thread _jobThread;

        /// <summary>
        /// 클라이언트가 ENTER 패킷을 제대로 보내고 있는지 확인하는 쓰레드
        /// 지정 된 시간 안에 보내지 않는다면 강제 접속 종료한다.
        /// </summary>
        private readonly Thread _waitThread;

        public int JobQueueCount => _jobQueue.Count;

        public int WaitQueueCount => _waitQueue.Count;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly ILogger _logger;

        public JobScheduler(Server server, ILoggerFactory loggerFactory)
        {
            _server = server;
            _jobQueue = new BlockingCollection<IJob>();
            _jobThread = new Thread(Job);

            _waitQueue = new ConcurrentQueue<(DateTime, IClient)>();
            _waitThread = new Thread(Wait);

            _logger = loggerFactory.CreateLogger<JobScheduler>();
        }

        private void Job(object id)
        {
            _logger.LogInformation($"[{id}] job thread start");
            var sw = new Stopwatch();
            while (!_jobQueue.IsCompleted) {
                try {
                    var job = _jobQueue.Take(cts.Token);
                    sw.Restart();
                    DoingJob(job);
                    sw.Stop();
                    if (sw.Elapsed.TotalSeconds > 1) {
                        _logger.LogWarning($"[{id}] DoingJob too slow - ({job.Type}) {sw.Elapsed.TotalSeconds} sec");
                    }
                } catch (OperationCanceledException) {
                    // ignore
                    //_logger.LogInformation($"[{id}] job queue thread end");
                } catch (Exception e) {
                    _logger.LogError($"[{id}] job queue fail - {e}");
                }
            }

            _logger.LogInformation($"[{id}] job queue thread end");
        }

        private void DoingJob(IJob job)
        {
            switch (job.Type) {
                case EJob.Packet: {
                        var pJob = (PacketJob)job;
                        _server.Dispatcher.DispatchPacket(pJob.Client, pJob.PacketId, pJob.Packet);
                    }
                    break;
                case EJob.Action: {
                        var aJob = (ActionJob)job;
                        aJob.Action();
                    }
                    break;
                case EJob.Propagate: {
                        var pJob = (PropagateJob)job;
                        _server.Propagate(pJob.Propagate, pJob.Client);
                    }
                    break;
                default:
                    _logger.LogError($"unknown job type - {job.Type}"); break;
            }
        }

        /// <summary>
        /// 다른 스레드(네트워크, 커맨드 등)에서 메인 스레드로 Job을 보내야 하는 경우 사용
        /// </summary>
        /// <param name="job"></param>
        public void AddJob(IJob job)
        {
            _jobQueue.Add(job);
        }

        public void AddWait(IClient client)
        {
            _waitQueue.Enqueue((DateTime.Now, client));
        }

        /// <summary>
        /// 지정 된 시간 안에 Enter 패킷을 보내지 않는다면 강제로 접속을 종료 시킨다
        /// </summary>
        private void Wait()
        {
            _logger.LogInformation("wait thread start");
            while (!_jobQueue.IsCompleted) {
                // waitQueue에 Count가 쌓여있을 경우 Thread.Sleep은 하지 않는다
                if (_waitQueue.Count > 0) {
                    if (_waitQueue.TryDequeue(out var items)) {
                        var client = items.client;
                        if (client != null) {
                            //if (client.State == Sockets.NetworkState.Login) {
                            //    var time = items.regTime;
                            //    var elapsedTime = (DateTime.Now - time).TotalSeconds;
                            //    if (elapsedTime > 10) {
                            //        client.Disconnect("Forced Disconnect to JobScheduler.Wait Function");
                            //    } else {
                            //        // waitQueue에 처음 Peek()이 다음 큐에 있는 인원에도 영향을 끼치지 않도록
                            //        // Dequeue 후에 다시 Enqueue 한다
                            //        _waitQueue.Enqueue(items);
                            //    }
                            //}
                        }
                        // state == enter : ingnore, 정상 처리
                        // state == closing, closed : client의 소켓이 닫힘 상태. Wait에서는 더 이상 할 것 없음
                    }
                } else {
                    Thread.Sleep(100);
                }
            }
            _logger.LogInformation("wait thread end");
        }

        public void Start()
        {
            _jobThread.Start(0);
            _waitThread.Start();
        }

        public void Stop()
        {
            _jobQueue.CompleteAdding();
            cts.Cancel();
        }
    }
}
