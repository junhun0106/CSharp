using System;

namespace Interfaces.Requests
{
    [CustomDTO(Url = "/test", Method = "GET")]
    [Serializable]
    public class TestRequest
    {
        public string Message { get; set; }

        [Serializable]
        public class Response
        {
            public string Message { get; set; }
        }
    }
}
