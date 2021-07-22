using System.Threading.Tasks;
using Grpc.Core;
using gRPCLib.Convert;
using gRPCLib.Proto;

namespace WebServer2
{
    public class Services : gRPCService.gRPCServiceBase
    {
        public override async Task<PingRequest.Types.Response> RpcPingRequest(PingRequest request, ServerCallContext context)
        {
            await Task.Delay(0);
            var response = new gRPCLib.Models.PingRequest.Response();

            System.Console.WriteLine($"ping - {gRPCLib.Models.Ports.Port2}");

            return response.Convert();
        }
    }
}
