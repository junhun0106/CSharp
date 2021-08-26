# StringParseWithSpan

* [참고 자료](https://gist.github.com/bbartels/87c7daae28d4905c60ae77724a401b20)

<img src="https://github.com/junhun0106/CSharp/blob/main/Span/benchmark_result.png">

---

* Span<T>는 ref readonly struct이며, ref readonly struct는 절대 heap에 할당 될 수 없도록 제한 되어 있다

* 따라서, ref readonly struct는 한 스코프 내에서만 사용하고 stack 메모리에서 해제 된다
    * 함수 내에서 잠깐 사용하기 위해 class 혹은 struct를 사용하는 경우에는 **ref readonly struct를 활용**하자 

* stack 메모리에서만 사용하기 때문에 훨씬 빠르게 접근 가능
* split에 경우 최소 n개의 string을 할당하고 배열까지 heap 영역에 할당
* split을 사용하더라도 자주 호출 되는 경우는 **최소 separator를 static readonly**로 만들자

---

###### 결과


| Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|------- |---------:|---------:|---------:|-------:|----------:|
|  Split | 55.50 ns | 0.536 ns | 0.501 ns | 0.0124 |     104 B |
|   Span | 17.17 ns | 0.244 ns | 0.229 ns | 0.0038 |      32 B |
| SubString | 20.07 ns | 0.137 ns | 0.128 ns | 0.0038 |      32 B |

---

##### TEST CODE

```
static char[] separator = new char[] { '@' };
string testString = "123@str";

[Benchmark]
public void Split()
{
    var split = testString.Split(separator);
    var s1 = split[0];
}

[Benchmark]
public void Span()
{
    var span = testString.AsSpan();
    var index = span.IndexOfAny(separator);
    var s1 = span.Slice(0, index).ToString();
}

[Benchmark]
public void SubString()
{
    var index = testString.IndexOfAny(separator);
    var s1 = testString.Substring(0, index);
}

```
