#### int.Parse

* dotnet 버전이 낮은 경우 int.Parse를 최적화 해보자

* int.Parse를 구현 할 때 필요한 부분만 구현하면 성능이 빨라진다
	* 그러나 메모리 사용은 줄어들지 않는다
	* 또 int.Parse가 보장하는 부분을 모두 구현한다면 속도가 느려지게 된다(int.Parse와 같아지게 된다)
	
* 결국 dontet이 낮은 버전에서 ReadOnlySpan<char>를 받아서 int.Parse를 할 수 있도록 해야 한다