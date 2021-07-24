# CSharp TCP Sample Server

### C#을 이용하여 간단한 채팅 서버 구현

##### 서버
>  * System.IO.Pipelines 사용
>  * [참고](https://devblogs.microsoft.com/dotnet/system-io-pipelines-high-performance-io-in-net/)
>  * System.IO.Pipelines에 최대 장점은 메모리 사용이 뛰어남(ReadOnlySequnece) 단점은 대부분의 구현 방법이 Task를 많이 사용함
>  * [System.IO.Pipelines 사용 후기](http://leafbird.github.io/devnote/2020/12/27/C-%EA%B3%A0%EC%84%B1%EB%8A%A5-%EC%84%9C%EB%B2%84-System-IO-Pipeline-%EB%8F%84%EC%9E%85-%ED%9B%84%EA%B8%B0/)처럼 장점(메모리 사용)만 사용하는 경우도 있음
>  * 혹은 [링크](https://github.com/davidfowl/TcpEcho/blob/master/src/Server/Program.cs)처럼 Task의 개수를 최대한 줄여서 사용하는 경우도 있음

---

> * BlockingCollection
> * 지정된 흐름에서 블로킹 되어 있다가 특정 액션(Add, Remove, Update)이 들어 왔을 경우에만 함수 처리
> * 그 외에 ReadOnly(보통 메세지를 보내서 브로드캐스트하는 경우)는 네트워크 스레드에서 바로 처리
 
---
 
* TODO : 어떤 방법이던 C# TCP Server는 조금 더 연구가 필요하다
* TODO : 수평 확장은 어떻게 ? 레디스의 pub/sub을 이용하여 하나의 패킷을 다른 서버에게 전달하는 방법. 더 좋은 방법은 없을까 ? (릴레이 서버는 구현보다는 이게 나은가?) 

---

* 참고 
 * 간단한 채팅 서버라고 되어 있지만, 테스트 프로젝트를 만들 당시에도 이미 월드 채널, 길드 채널 채팅방 등 다양한 채팅 콘텐츠가 기획 되어 있었기에 고려하여 작성
 * 실제 채팅 서버 구현은 테스트 프로젝트를 바탕으로 확장시킴

##### 클라이언트
 * 클라이언트는 C#에서 주로 사용하는 SocketAsyncEventArg 방식을 사용 
