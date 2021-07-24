using ChatService.Clients;
using ChatService.Packets;
using Interfaces;

namespace ChatService
{
    public interface IDispatcher
    {
        PacketHandler Dispatcher { get; }
        void AddPacket(IClient client, string packetId, PacketServerBase packet);
    }

    public interface IJobSchedule
    {
        /// <summary>
        /// 다른 스레드(커맨드, 네트워크 등)에서 메인 스레드(JobScheduler)로 Job을 보내야 하는 경우
        /// </summary>
        void AddJob(IJob job);

        /// <summary>
        /// 이미 메인 스레드인 경우 곧바로 액션을 취해야 하는 경우
        /// </summary>
        void DoingJob(EPropagate propagate, IClient client);

        /// <summary>
        /// 이미 메인 스레드인 경우 곧바로 액션을 취해야 하는 경우
        /// </summary>
        void DoingJob(IPropagate propagate, IClient client);

        void AddWait(IClient client);
    }

    public interface IServer : IJobSchedule, IDispatcher
    {
        /// <summary> 채팅 메세지를 필요한 매니저에 전달 한다 </summary>
        void Message(Chat chat);
    }
}
