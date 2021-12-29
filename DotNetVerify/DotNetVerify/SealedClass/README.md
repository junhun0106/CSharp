# 봉인 클래스 벤치 마크

* .NET 6 이 전 버전 테스트 결과
	* [Wiki](https://github.com/junhun0106/CSharp/wiki/%5BOptimize%5D-sealed-class)
	* [테스트 프로젝트](https://github.com/junhun0106/CSharp/tree/main/StudyProject/SealedClassBenchMark)
	* .NET 6 전 버전에서는 성능의 차이를 확인 할 수 없었음
	* Attribute도 마찬가지


#### 봉인 된 Attribute 테스트

|     Method |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
|----------- |---------:|--------:|--------:|-------:|----------:|
| NoneSealed | 449.1 ns | 2.79 ns | 2.61 ns | 0.0062 |     104 B |
|     Sealed | 503.2 ns | 2.81 ns | 2.63 ns | 0.0057 |     104 B |

#### Sealed Class Function Call

* VT를 부르지 않는지 ?