#### [sealed class](https://github.com/junhun0106/CSharp/wiki/%5BOptimize%5D-sealed-class) 

## CASE 1

* 상속을 하지 않았거나, 인터페이스를 상속 받았어도 override 된 함수를 사용하지 않은 경우에는 크게 문제가 없다
* 참고 : Call 함수를 너무 간단하게 작성하면 BenchMarkDotNet이 제대로 된 시간을 찾아주지 않음

---

#### TEST 1 : 상속 받지 않은 일반 클래스와 sealed 클래스 테스트

|      Method |     Mean |    Error |   StdDev |
|------------ |---------:|---------:|---------:|
| CallRegular | 41.79 ns | 0.200 ns | 0.187 ns |
|  CallSealed | 41.77 ns | 0.167 ns | 0.148 ns |

```
public class AAA
{
    public string Call()
    {
        var logger = nameof(AAA);
        return $"{logger} call {nameof(Call)}";
    }
}

public sealed class BBB 
{
    public string Call()
    {
        var logger = nameof(BBB);
        return $"{logger} call {nameof(Call)}";
    }
}

[Benchmark]
public void CallRegular()
{
    aaa.Call();
    aaa.Call();
    aaa.Call();
}

[Benchmark]
public void CallSealed()
{
    bbb.Call();
    bbb.Call();
    bbb.Call();
}
```


#### TEST 2 : 인터페이스를 재정의한 일반 클래스와 sealed 클래스 테스트

|      Method |     Mean |    Error |   StdDev |
|------------ |---------:|---------:|---------:|
| CallRegular | 42.63 ns | 0.301 ns | 0.281 ns |
|  CallSealed | 42.58 ns | 0.149 ns | 0.132 ns |

```
interface IInterface
{
    string Call();
}

public class AAA : IInterface
{
    public string Call()
    {
        var logger = nameof(AAA);
        return $"{logger} call {nameof(Call)}";
    }
}

public sealed class BBB : IInterface
{
    public string Call()
    {
        var logger = nameof(BBB);
        return $"{logger} call {nameof(Call)}";
    }
}

[Benchmark]
public void CallRegular()
{
    aaa.Call();
    aaa.Call();
    aaa.Call();
}

[Benchmark]
public void CallSealed()
{
    bbb.Call();
    bbb.Call();
    bbb.Call();
}
```

---

## CASE 2

---

#### TEST 1 : ToString을 재정의 한 일반 클래스와 sealed 클래스

|      Method |     Mean |    Error |   StdDev |
|------------ |---------:|---------:|---------:|
| CallRegular | 44.73 ns | 0.378 ns | 0.353 ns |
|  CallSealed | 42.63 ns | 0.225 ns | 0.210 ns |

|      Method |     Mean |    Error |   StdDev |
|------------ |---------:|---------:|---------:|
| CallRegular | 44.90 ns | 0.446 ns | 0.372 ns |
|  CallSealed | 42.96 ns | 0.195 ns | 0.182 ns |

```
public class AAA
{
    public override string ToString()
    {
        var logger = nameof(AAA);
        return $"{logger} call {nameof(ToString)}";
    }
}

public sealed class BBB
{
    public override string ToString()
    {
        var logger = nameof(BBB);
        return $"{logger} call {nameof(ToString)}";
    }
}
 [Benchmark]
public void CallRegular()
{
    aaa.ToString();
    aaa.ToString();
    aaa.ToString();
}

[Benchmark]
public void CallSealed()
{
    bbb.ToString();
    bbb.ToString();
    bbb.ToString();
}
```

#### TEST 2 : 가상 함수 Call을 재정의 한 일반 클래스와 sealed 클래스


|      Method |     Mean |    Error |   StdDev |
|------------ |---------:|---------:|---------:|
| CallRegular | 44.82 ns | 0.233 ns | 0.218 ns |
|  CallSealed | 43.03 ns | 0.162 ns | 0.152 ns |

|      Method |     Mean |    Error |   StdDev |
|------------ |---------:|---------:|---------:|
| CallRegular | 46.19 ns | 0.837 ns | 0.742 ns |
|  CallSealed | 45.58 ns | 0.930 ns | 1.629 ns |

|      Method |     Mean |    Error |   StdDev |
|------------ |---------:|---------:|---------:|
| CallRegular | 44.78 ns | 0.199 ns | 0.186 ns |
|  CallSealed | 43.50 ns | 0.212 ns | 0.198 ns |

```
public class AAA
{
    public virtual string Call()
    {
        return nameof(AAA);
    }
}

public class BBB : AAA
{
    public override string Call()
    {
        var logger = nameof(BBB);
        return $"{logger} child from {nameof(AAA)}";
    }
}

public sealed class CCC : AAA
{
    public override string Call()
    {
        var logger = nameof(CCC);
        return $"{logger} child from {nameof(AAA)}";
    }
}
        [Benchmark]
public void CallRegular()
{
    bbb.Call();
    bbb.Call();
    bbb.Call();
}

[Benchmark]
public void CallSealed()
{
    ccc.Call();
    ccc.Call();
    ccc.Call();
}
```

---

## 결론

* Attribute는 sealed class를 사용하자
* 재정의 된(override) class가 많다면, selaed class를 사용하자(script)

* 우리 프로젝트에서 자주 사용되는 케이스(자주 생성되고, 자주 호출되는 경우)
    * Attribute(The class is an attribute that requires very fast runtime look-up. Sealed attributes have slightly higher performance levels than unsealed ones, DO seal custom attribute classes, if possible. This makes the look-up for the attribute faster)
    * 스크립트
    * 메세지
    * Exception

* **But**
    * [성능에 대한 이점](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/abstract-and-sealed-classes-and-class-members
        )이 엄청 큰 것도 아니고, 사용해야하는 부분도 제한적이다
        * MSDN에서는 찾지 못했지만, sealed class의 경우 더 이상 '가상 함수 테이블(V-Table)'을 사용 할 필요가 없기 때문으로 알려져 있다
            * callvirt -> call
        * 봉인 클래스는 기본 클래스로 사용될 수 없으므로 일부 런타임 최적화에서는 봉인 클래스 멤버 호출이 약간 더 빨라집니다(Sealed classes prevent derivation. Because they can never be used as a base class, some run-time optimizations can make calling sealed class members slightly faster)
        * [MSDN에서도 적합한 이유가 없다면 봉인(sealing)을 하지 말라고 되어 있다](https://docs.microsoft.com/ko-kr/dotnet/standard/design-guidelines/sealing)
    * 단, Attribute는 MSDN에서 확실히 sealed 클래스를 사용하면 성능에 대한 이점이 높아진다고 되어 있다
    * 단, 재정의하는 멤버는 봉인하는 것이 좋다고 설명하고 있다
    * static class는 이미 [sealed abstract class](https://docs.microsoft.com/ko-kr/dotnet/standard/design-guidelines/static-class) 이다

---

CASE2 - TEST2를 아래와 같이 변형하여 사용해도 아주 약간의 성능 이점이 있다

|      Method |     Mean |    Error |   StdDev |
|------------ |---------:|---------:|---------:|
| CallRegular | 45.39 ns | 0.669 ns | 0.626 ns |
|  CallSealed | 43.67 ns | 0.310 ns | 0.242 ns |

```
public class CCC : AAA
{
    public sealed override string Call()
    {
        var logger = nameof(CCC);
        return $"{logger} child from {nameof(AAA)}";
    }
}
```    

---

