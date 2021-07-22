using System.Threading.Tasks;
using Grpc.Core;
using gRPCLib.Convert;
using gRPCLib.Proto;

namespace WebServer1
{
    public class Services : gRPCService.gRPCServiceBase
    {
        public override async Task<PingRequest.Types.Response> RpcPingRequest(PingRequest request, ServerCallContext context)
        {
            await Task.Delay(0);
            var response = new gRPCLib.Models.PingRequest.Response();

            System.Console.WriteLine($"ping - {gRPCLib.Models.Ports.Port1}");

            return response.Convert();
        }
    }
}
