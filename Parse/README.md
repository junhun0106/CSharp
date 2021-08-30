#### int.Parse

* dotnet 버전이 낮은 경우 int.Parse를 최적화 해보자
* NumberStyles.Integer에서 필요한 선후행 공백, 선행 부호만 처리해주면 된다


#### bool.Parse

* bool.Parse는 실제 코드도 매우 간편하다.
* 불필요한 if, trace를 제거한 코드를 사용해보자.

#### double.Parse

* Integer와 같음
	* AllowLeadingWhite, AllowTrailingWhite : 선후행 공백
	* AllowLeadingSign : 선행 부호만
	
* AllowDecimalPoint
	* Integer처럼 무시하되, decimalPointCount만 기억해두자
	
* AllowExponent
	* 'E' 또는 'e'
	* 지수를 counting 해놓자
	
* AllowThousands
	* ','
	* 그룹 기호도 parsing 될 수 있게 해야 한다
	* 재화 표기가 아니라면 default는 ',' 밖에 없다.
	* break가 아닌 무시 처리 하자
	
* 정수값이 가장 큰 ulong 형태로 값을 추출
* 지수값 - decimalPoint를 마지막에 pow 해준다


|                  Method |       Mean |    Error |   StdDev |  Gen 0 | Allocated |
|------------------------ |-----------:|---------:|---------:|-------:|----------:|
|             DoubleParse | 4,048.5 ns | 41.76 ns | 39.06 ns | 0.0839 |     720 B |
| DoubleSpanParseInternal | 3,631.5 ns | 25.60 ns | 23.95 ns |      - |         - |
|   DoubleSpanParseCustom |   985.4 ns |  4.85 ns |  4.30 ns |      - |         - |


 * 추가로 double.Parse(string)과 DoubleParser.Parse(string.AsSpan)도 Custom 함수가 더 빠른 것을 볼 수 있다
 * 안전성이 보장된다면, IESObject.Get, GetOrDefault에서도 사용하면 좋을 것 같다

|                  Method |       Mean |    Error |   StdDev |  Gen 0 | Allocated |
|------------------------ |-----------:|---------:|---------:|-------:|----------:|
|       DoubleDirectParse | 3.730 us | 0.0103 us | 0.0097 us |      - |         - |
|   DoubleDirectSpanParse | 1.043 us | 0.0031 us | 0.0029 us |      - |         - |