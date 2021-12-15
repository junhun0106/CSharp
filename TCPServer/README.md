# CSharp TCP Sample Server

### C#을 이용하여 간단한 채팅 서버 구현

##### 서버
>  * System.IO.Pipelines 사용
>  * [참고](https://devblogs.microsoft.com/dotnet/system-io-pipelines-high-performance-io-in-net/)
>  * System.IO.Pipelines에 최대 장점은 메모리 사용이 뛰어남(ReadOnlySequnece) 단점은 대부분의 구현 방법이 Task를 많이 사용함
>  * [System.IO.Pipelines 사용 후기](http://leafbird.github.io/devnote/2020/12/27/C-%EA%B3%A0%EC%84%B1%EB%8A%A5-%EC%84%9C%EB%B2%84-System-IO-Pipeline-%EB%8F%84%EC%9E%85-%ED%9B%84%EA%B8%B0/)처럼 장점(메모리 사용)만 사용하는 경우도 있음
>  * 혹은 [링크](https://github.com/davidfowl/TcpEcho/blob/master/src/Server/Program.cs)처럼 Task의 개수를 최대한 줄여서 사용하는 경우도 있음
>  https://source.dot.net/#Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets/Internal/SocketConnection.cs,6428be11c3ed3b0f

---

> * BlockingCollection
> * 지정된 흐름에서 블로킹 되어 있다가 특정 액션(Add, Remove, Update)이 들어 왔을 경우에만 함수 처리
> * 그 외에 ReadOnly(보통 메세지를 보내서 브로드캐스트하는 경우)는 네트워크 스레드에서 바로 처리

---

* TODO : 어떤 방법이던 C# TCP Server는 조금 더 연구가 필요하다
* TODO : 수평 확장은 어떻게 ? 레디스의 pub/sub을 이용하여 하나의 패킷을 다른 서버에게 전달하는 방법. 더 좋은 방법은 없을까 ? (릴레이 서버는 구현보다는 이게 나은가?) 

---

* 참고 
> * 간단한 채팅 서버라고 되어 있지만, 테스트 프로젝트를 만들 당시에도 이미 월드 채널, 길드 채널 채팅방 등 다양한 채팅 콘텐츠가 기획 되어 있었기에 고려하여 작성
> * 실제 채팅 서버 구현은 테스트 프로젝트를 바탕으로 확장시킴

---

##### 클라이언트
 * 클라이언트는 C#에서 주로 사용하는 SocketAsyncEventArg 방식을 사용 

---

### C#으로 구현한 다른 서버의 경우

* 실시간 게임 서버
> * 클라이언트 혹은 필드를 기점으로 패킷을 한 곳으로 모으고, lock 실수를 최대한 방지.
> * 서로 간의 간섭이 없는 곳(스레드 컨디션이 일어날 수 없는 곳)을 페러렐 for를 이용하여 성능을 가져옴
>> * 한 필드로 묶는다고 가정하면 필드 내에 클라이언트들은 같은 스레드 내에 있기 때문에 스레드 컨디션(혹은 레이스 컨디션)이 일어나지 않음
> * 서로 간의 간섭이 일어나는 경우(필드에서 필드로 이동하는 경우)는 필드를 묶는 상위 매니저에서 동기화 처리

* 매칭 서버
> * 매칭 서버는 connection과 disconnect이 잦은 서버이고, connection이 오래 유지되지 않음
> * 따라서, 서버에 연결 되어 있는 클라이언트 개수가 적으므로 안전하게 메인 루프로 패킷을 모음
> * 내부 테스트 혹은 퍼블리셔 테스트에서도 크게 문제 되지 않음

---

#### history

* 2021-8-31
	* 불필요한 Task.Delay 제거
	* 최대한 Task 사용 제한
	* CPU 점유율 감소(20~30%)
	* TODO : 패킷 송신에서 PipeWriter, PipeReader가 아닌 버퍼 재사용만 간단하게 구현 가능하도록 하자 

* 2021-9-2
	* Pipe 최적화
		* SlabMemoryPool 활용(Kestrel 오픈 소스에서 자주 사용하는 것 발견, SlabMemory 이론 참고)
			* .NET 5.0에서는 PinnedBlockMemoryPool 사용
			* 일반적으로 메모리 고정을 할 수 있는 GCHandle(or fixed)에서 GC.AllocateUninitializedArray 함수가 추가
	* 메모리 최적화
		* 최대한 Span 상태에서 해결 가능 하도록
		* ValueStringBuilder 사용(StringBuilderPool 보다 약간 빠름(readonly ref struct), Kestrel 오픈 소스에서 자주 사용하는 것 발견)
		* System.Text.Json 사용(Span 활용, Json 벤치마크 참고)
	* 기타
		* 네이밍 변경
		* sealed class
