### ASP.NET CORE gRPC FailOver

* 웹 서버의 이상 현상이 감지 되었을 때, 다른 gRPC 채널로 이동 시킨다
  * 간단한 웹 요청을 지속적으로 보내는 클라이언트에서 지정 된 오류가 발생 했을 때, 다른 gRPC 채널로 이동하여 웹 요청을 시도한다