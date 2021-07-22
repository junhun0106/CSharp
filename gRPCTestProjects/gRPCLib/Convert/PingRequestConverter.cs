using System;
using System.Collections.Generic;
using System.Text;

namespace gRPCLib.Convert
{
    using Origin = gRPCLib.Models;
    using gRPC = gRPCLib.Proto;

    public static class PingRequestConverter
    {
        public static Origin.PingRequest Convert(this gRPC.PingRequest _)
        {
            return new Origin.PingRequest();
        }

        public static Origin.PingRequest.Response Convert(this gRPC.PingRequest.Types.Response _)
        {
            return new Origin.PingRequest.Response();
        }

        public static gRPC.PingRequest Convert(this Origin.PingRequest _)
        {
            return new gRPC.PingRequest();
        }

        public static gRPC.PingRequest.Types.Response Convert(this Origin.PingRequest.Response _)
        {
            return new gRPC.PingRequest.Types.Response();
        }
    }
}
