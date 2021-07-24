using Interfaces;
using Interfaces.Requests;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace HttpTester
{
    class Program
    {
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        public static HttpRequestMessage GetRequest_Http(object dto)
        {
            var type = dto.GetType();
            var attr = (CustomDTOAttribute)type.GetCustomAttributes(typeof(CustomDTOAttribute), false)[0];
            var apiUri = attr.Url;
            HttpMethod method;
            bool requestBody = false;
            switch (attr.Method) {
                case "GET": method = HttpMethod.Get; break;
                case "POST": method = HttpMethod.Post; requestBody = true; break;
                case "PUT": method = HttpMethod.Put; requestBody = true; break;
                case "DELETE": method = HttpMethod.Delete; break;
                default: method = HttpMethod.Get; break;
            }

            if (requestBody) {
                var request = new HttpRequestMessage(method, apiUri);
                var json = JsonConvert.SerializeObject(dto, _jsonSettings);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                return request;
            } else {
                var props = type.GetProperties();
                var fields = type.GetFields();
                if (props.Length == 0 && fields.Length == 0) {
                    return new HttpRequestMessage(method, apiUri);
                }
                var sb = new StringBuilder();
                if (fields.Length > 0) {
                    foreach (var field in fields) {
                        var name = field.Name;
                        var value = field.GetValue(dto).ToString();
                        if (sb.Length == 0) {
                            sb.Append("?");
                        } else {
                            sb.Append("&");
                        }
                        sb.Append(name).Append("=").Append(HttpUtility.UrlDecode(value));
                    }
                }

                if (props.Length > 0) {
                    foreach (var prop in props) {
                        var name = prop.Name;
                        var value = prop.GetValue(dto).ToString();
                        if (sb.Length == 0) {
                            sb.Append("?");
                        } else {
                            sb.Append("&");
                        }
                        sb.Append(name).Append("=").Append(HttpUtility.UrlDecode(value));
                    }
                }
                return new HttpRequestMessage(method, apiUri + sb.ToString());
            }
        }

        static void Main(string[] args)
        {
            try {
                using (var httpClient = new HttpClient()) {
                    // keep-alive = true
                    httpClient.BaseAddress = new Uri("http://localhost:5000");
                    httpClient.DefaultRequestHeaders.ConnectionClose = false;
                    // 인증 서버 혹은 세션 서버에서 받은 Token을 채운다
                    // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "abcd");

                    var request = GetRequest_Http(new TestRequest {
                        Message = "httpClient_tester",
                    });

                    var r = httpClient.SendAsync(request).Result;
                    var json = r.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<TestRequest.Response>(json);
                    Console.WriteLine(response.Message);
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            
            Console.WriteLine("complete test...");
            Console.ReadLine();
        }
    }
}
