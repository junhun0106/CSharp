using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace LatencyView
{
    public class StatisticsMiddleware
    {
        private readonly StatisticsData data;
        private readonly RequestDelegate next;

        public StatisticsMiddleware(RequestDelegate next, StatisticsData data)
        {
            this.next = next;
            this.data = data;
        }

        public async Task Invoke(HttpContext context)
        {
            var now = System.DateTime.UtcNow;
            var sw = Stopwatch.StartNew();

            await next(context).ConfigureAwait(false);

            sw.Stop();
            var elapsed = sw.Elapsed;

            // close request
            var url = context.Request.Path.Value;
            var method = context.Request.Method;
            bool isStatusOk = context.Response.StatusCode == (int)HttpStatusCode.OK;

            data.Add(url, method, isStatusOk, elapsed);
            
            LatencyLogger.Log(now, System.DateTime.UtcNow, isStatusOk, $"[{context.Request.Method}]{context.Request.Path}");
        }
    }
}
