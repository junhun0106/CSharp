namespace ChattingMultiTool
{
    public class Config
    {
        /// <summary>
        /// 채팅 서버 IP
        /// </summary>
        public string Ip = "127.0.0.1";

        /// <summary>
        /// 채팅 서버 포트
        /// </summary>
        public int Port = 15344;

        public string LogPath = "../../../../../../logs/ChattingMultilTool/chatting_multi_tool.log";

        /// <summary>
        /// 테스트를 실행 할 클라이언트 수
        /// </summary>
        public int ClientCount = 1;

        /// <summary>
        /// 1초에 입장하는 클라이언트 개수
        /// </summary>
        public int EnterPerSec = 100;

        /// <summary>
        /// 1초에 보내는 패킷 수, 시나리오에 하나에 보통 1개의 패킷을 보내므로 많이 잡을 필요 없다
        /// </summary>
        public int SendPacketPerSec = 1;

        /// <summary>
        /// 실행 될 시나리오
        /// </summary>
        public int ScenarioNum = (int)Scenario.EScenario.Chatting;
    }
}
