## 정적 로컬 함수

* 캡처되는 변수가 없는 경우 static local function 사용 가능. 
* static local function은 힙 할당을 하지 않는다.
  * 정말 할당을 하지 않는지 확인해보자
  * https://docs.microsoft.com/ko-kr/dotnet/csharp/programming-guide/classes-and-structs/local-functions#heap-allocations

---

## List<T>.Select 테스트

* 1차 테스트 결과

|                     Method |     Mean |    Error |   StdDev |   StdErr |      Min |       Q1 |   Median |       Q3 |      Max |         Op/s |  Gen 0 | Allocated |
|--------------------------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|-------------:|-------:|----------:|
|                       Func | 42.23 ns | 0.369 ns | 0.308 ns | 0.086 ns | 41.69 ns | 42.04 ns | 42.21 ns | 42.38 ns | 42.72 ns | 23,678,307.3 | 0.0091 |     152 B |
|                  LocalFunc | 42.87 ns | 0.595 ns | 0.556 ns | 0.144 ns | 42.13 ns | 42.49 ns | 42.74 ns | 43.33 ns | 43.96 ns | 23,324,422.0 | 0.0091 |     152 B |
|       LocalFuncInParameter | 54.70 ns | 0.297 ns | 0.248 ns | 0.069 ns | 54.31 ns | 54.47 ns | 54.77 ns | 54.85 ns | 55.03 ns | 18,282,462.5 | 0.0167 |     280 B |
|            StaticLocalFunc | 42.13 ns | 0.325 ns | 0.288 ns | 0.077 ns | 41.70 ns | 41.85 ns | 42.16 ns | 42.33 ns | 42.58 ns | 23,736,932.9 | 0.0091 |     152 B |
| StaticLocalFuncInParameter | 54.15 ns | 0.553 ns | 0.462 ns | 0.128 ns | 53.01 ns | 53.97 ns | 54.28 ns | 54.31 ns | 54.91 ns | 18,467,312.6 | 0.0167 |     280 B |
|                 StaticFunc | 43.31 ns | 0.890 ns | 1.360 ns | 0.244 ns | 41.20 ns | 42.35 ns | 42.94 ns | 44.03 ns | 46.19 ns | 23,091,693.5 | 0.0091 |     152 B |
|      StaticFuncInParameter | 54.76 ns | 1.093 ns | 1.215 ns | 0.279 ns | 53.23 ns | 53.71 ns | 54.60 ns | 55.66 ns | 57.71 ns | 18,260,305.9 | 0.0167 |     280 B |


* 의문점1 : static 함수와의 차이점 ?
  * MSDN에서는 코드의 의도를 명확하게 하기 위함. 특정 함수가 해당 함수 안에서만 사용하는 것이라는 표현

* 파라미터로 넣었을 경우 더 느리게 동작하는 것 확인
* 할당은 똑같은데 ?

---

* 2차 테스트

* LocalFuncInParameter, StaticLocalFuncInParameter, StaticFuncInParameter
  * 확실히 더 느리고, Allocated가 많아서 테스트에서 제외
* StaticFunc
  * StaicLocalFunc과 결과가 같아서 테스트에서 제외
 
|          Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|---------------- |---------:|---------:|---------:|-------:|----------:|
|            Func | 41.18 ns | 0.423 ns | 0.395 ns | 0.0091 |     152 B |
|       LocalFunc | 41.79 ns | 0.291 ns | 0.272 ns | 0.0091 |     152 B |
| StaticLocalFunc | 42.00 ns | 0.360 ns | 0.319 ns | 0.0091 |     152 B |

* static으로 변환 할 수 있는 Func, 로컬 함수의 경우에는 모두 같은 할당을 한다.  

---

* 3차 테스트 결과

* 로컬 함수 MSDN에 경우 struct 혹은 value type들에 대해서 언급하고 있다

```
로컬 함수는 struct를 사용할 수 있는 반면, 람다는 항상 class를 사용합니다.
```

* class를 struct로 변경한 다음에 테스트를 하고
* StaticLocalFunc
  * static으로 선언 할 수 있는 경우는 할당이 같으므로 테스트에서 제외
  * static 함수는 명시적인 선언

---

결론

* static 로컬 함수가 될 수 있을 때 명시적으로 선언하는 것을 권장
  * 결과를 놓고 봤을 땐 취향 차이
  * 컴파일러 에러
  * static 로컬 함수에 캡처가 필요한 경우를 다시 한 번 생각 해 볼 수 있음

* struct와 같은 value type이 함께 포함 되어 캡처를 할 경우에는 local function이 유리
  * 아주 근소하지만 빠름

* local function을 사용 할 경우 반드시 (x => local_func(x)) 형태로 파라미터를 넘겨야 한다

* 하지만 힙 할당은 똑같다!!
