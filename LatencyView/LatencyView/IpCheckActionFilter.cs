using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

namespace LatencyView
{
    public class IpCheckActionFilter : ActionFilterAttribute
    {
        private readonly ILogger<IpCheckActionFilter> _logger;
        private readonly string[] _whiteList;

        public IpCheckActionFilter(ILoggerFactory loggerFactory)
        {
            // whiteList는 appsettings로 관리하자
            // _whiteList = whiteList;
            _logger = loggerFactory.CreateLogger<IpCheckActionFilter>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            _logger.LogDebug("Remote IpAddress: {RemoteIp}", remoteIp);
            if (_whiteList?.Length > 0) {
                var badIp = true;
                if (remoteIp.IsIPv4MappedToIPv6) {
                    remoteIp = remoteIp.MapToIPv4();
                }
                foreach (var address in _whiteList) {
                    var testIp = IPAddress.Parse(address);
                    if (testIp.Equals(remoteIp)) {
                        badIp = false;
                        break;
                    }
                }
                if (badIp) {
                    _logger.LogWarning("Forbidden Request from IP: {RemoteIp}", remoteIp);
                    context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                    return;
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
