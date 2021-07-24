using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApplication2.Middleware
{
    public class TestMiddleware
    {
        private readonly RequestDelegate next;

        public TestMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await next(context).ConfigureAwait(false);
            sw.Stop();

            // 레이턴시 로그 수집
        }
    }
}
