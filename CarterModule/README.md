# Carter 라이브러리 활용

* Url, Method를 관리하기 쉽도록 Interface 프로젝트를 이용

* HttpClient와 Carter를 이용한 웹 서버와의 심플 통신 프로젝트

* [Carter](https://github.com/CarterCommunity/Carter)
> 주의 사항 
> Carter 구 버전을 사용 할 경우에는 DefaultJsonModelBinder는 NewtonSoft.Json을 사용 했다
> NewtonSoft.Json에 비동기 함수가 없다보니, IHttpBodyControlFeature.AllowSynchronousIO를 강제로 true로 변경 시켜준다
>> WebSurge를 이용하여, NewtonSoft.Json과 System.Text.Json의 BindOrDefault 비교
>> <img src="https://github.com/junhun0106/CSharp/blob/main/CarterModule/WebSurge_Json.png">

> 최근에 Carter 버전은 Carter에서도 DefaultJsonModelBinder에 System.Json.Text를 사용하는 걸로 보인다
>> BindOrDefault를 Custom 코드로 만들 경우 Dictionary에 대한 Converter도 만들어주어야 한다
