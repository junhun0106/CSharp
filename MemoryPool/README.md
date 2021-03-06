
* 참고 
	* [PipeOptions](https://source.dot.net/#System.IO.Pipelines/System/IO/Pipelines/PipeOptions.cs,92bd1c42d1d37e7a)
	* [DefaultMemoryPool(ArrayMemoryPool)](https://source.dot.net/#System.Memory/System/Buffers/ArrayMemoryPool.cs,3fa031cb76db3922)
	* [PinnedBlockMemoryPool](https://github.com/dotnet/aspnetcore/blob/main/src/Shared/Buffers.MemoryPool/PinnedBlockMemoryPool.cs)
	* [SlabMemoryPool](https://github.com/dotnet/aspnetcore/blob/2e1063ea5b7270a7cc5fade9980e868abd5f8cfe/src/Shared/Buffers.MemoryPool/SlabMemoryPool.cs)
		*[Slab Memory](https://en.wikipedia.org/wiki/Slab_allocation)
	* [고정(pinning)](https://docs.microsoft.com/ko-kr/dotnet/framework/interop/copying-and-pinning)
---

* C#에서 서버 송수신에서 가장 고민하게 되는 것은 크게 3가지가 있다
	* SocketAsyncEventArgs 방식
		* 버퍼 재사용에 관한 문제
	* 버퍼 재사용을 위한 솔루션 System.IO.Pipelines
		* 샘플 코드를 보면 Task를 왜 이렇게 많이 사용하지 ?
		* 프로파일링을 해보면 메모리 누수는 일어나지 않지만, 메모리 할당이 많아 GC Wait으로 인해 너무 자주 끊기네 ?
		
* 이 프로젝트는 메모리 할당을 많이 차지하는 이유에 대해서 연구해본다
	* 아직 Slab Memory와 메모리 고정 그리고 .net 5.0 이상부터 등장한 POH에 관한 연구가 덜 되었다.
	* 이론은 차차 공부해도록하고, 결과를 보면서 이야기해보자.

#### .net 5.0

* MemoryPool.Rent 테스트 결과

|           Method |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
|----------------- |---------:|--------:|--------:|-------:|----------:|
|  ArrayMemoryPool | 259.6 ns | 1.57 ns | 1.39 ns | 0.0286 |     240 B |
|   SlabMemoryPool | 146.7 ns | 0.86 ns | 0.81 ns |      - |         - |
| PinnedMemoryPool | 139.0 ns | 0.34 ns | 0.32 ns |      - |         - |

---

* **new Pipe(PipeOptions.Default)** 에서 사용하는 메모리 풀은 MemoryPool.Shared에 기본 메모리풀인 **ArrayMemoryPool** 이다.
* 편의상 아래와 같이 언급하겠다
	* ArrayMemoryPool은 default
	* SlabMemoryPool은 slab
	* PinnedBlockMemoryPool은 pinned
* 메모리 프로파일링은 해보면 default, slab, pinned에 메모리 차이를 볼 수 있다.
	* 처음에는 default와 slab만 비교 했을 때는 default가 2배 차이가 났다.
	* 후에 pinned를 추가하여 테스트 했을 때는 default와 slab이 비슷한 값이 나왔다.
		* 오히려 slab이 더 나왔는데, 이는 default에서 사용하는 ArrayMemoryPoolBuffer가 할당을 더 적게(멤버 변수가 적기) 하기 때문이다
		* IMemoryOwner.Dispose 해줘야 정상적인 테스트 결과를 얻을 수 있다
	* 잘못된 테스트 결과
		* <img src="https://github.com/junhun0106/CSharp/blob/main/MemoryPool/%EC%9E%98%EB%AA%BB%EB%90%9C_%ED%85%8C%EC%8A%A4%ED%8A%B8.PNG"> 
	* dispose 이후에 테스트 결과를 보자.
		* slab과 pinned는 메모리 할당을 거의 하지 않는다.
		* 코드 자체가 절차식으로 되어 있기 때문에 1개만 생성하여 돌려쓰기 때문이다
			* pipe 객체처럼 소켓 통신 등에 사용 할 때는 멀티스레드 환경이므로, 좀 더 많이 할당을 받는다
			* 하지만 항상 같은 할당을 보이는 default와 다르게 slab과 pinned는 재사용 할 가능성이 있기 때문에 상대적으로 메모리를 적게 쓰고 속도가 좋을 수 밖에 없다
		* <img src="https://github.com/junhun0106/CSharp/blob/main/MemoryPool/dipose_test.PNG"> 
	* default도 dispose 해주었는데 ?
		* default가 ArrayPool에서 가져온 부분은 반환하지만, ArrayMemoryPoolBuffer는 재사용하지 않기 때문이다.
		* 기존 memory pool들은 thread safe하게 만들어졌지만 테스트를 위해 재사용하는 ArrayMemoryPool을 만들어서 테스트해보자.
			* ArrayMemoryPoolBuffer 재사용에 대한 메모리 할당을 보기 위한 테스트이므로, custom array memory pool에 대한 안정성 보장은 하지 않았다
		* pinned array을 만들지 않고, arrayPool에서 빌려오기 때문에 저렴해진 것을 볼 수 있다
			* 단, 기존 MemoryPool.Shared는 generic이고 custom array memory pool은 byte만 받을 수 있는 차이가 있다
		* <img src="https://github.com/junhun0106/CSharp/blob/main/MemoryPool/custom.PNG"> 
	* slab과 pinned는 무슨 차이 ?
		* .net 5.0이 되면서 slab을 pinned로 변경되었다. 이 부분은 좀 더 연구를 해보자 !

---

* MemoryPool.Rent 테스트(+ custom array memory pool) 결과

|                Method |      Mean |    Error |   StdDev |  Gen 0 | Allocated |
|---------------------- |----------:|---------:|---------:|-------:|----------:|
|       ArrayMemoryPool | 256.78 ns | 0.515 ns | 0.482 ns | 0.0286 |     240 B |
| CustomArrayMemoryPool |  90.02 ns | 0.317 ns | 0.297 ns |      - |         - |
|        SlabMemoryPool | 141.89 ns | 0.544 ns | 0.454 ns |      - |         - |
|      PinnedMemoryPool | 138.51 ns | 0.560 ns | 0.524 ns |      - |         - |

* single thread 기반으로 만들고, 불필요한 예외 처리 등을 제거 했기 때문에 custom이 가장 빠른 것을 볼 수 있다.
	* 다만, PipeOption 등에 사용하려면 안정성 테스트와 Thread Safe하게 동작하는 지 테스트를 거쳐야 한다.
	* 즉, 테스트용으로 만들어졌기 때문에 굳이 사용 할 필요가 없고 Kestrel이라는 훌륭한 open source를 활용하자.		
