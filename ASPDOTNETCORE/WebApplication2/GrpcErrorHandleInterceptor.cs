//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Grpc;
//using Grpc.Core;
//using Grpc.Core.Interceptors;
//using Microsoft.Extensions.Logging;
//using MySql.Data.MySqlClient;

//namespace WebApplication2
//{
//    public class GrpcErrorHandleInterceptor : Interceptor
//    {
//        private readonly ILogger<GrpcErrorHandleInterceptor> _logger;
//        public GrpcErrorHandleInterceptor(ILogger<GrpcErrorHandleInterceptor> logger)
//        {
//            _logger = logger;
//        }

//        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
//        {
//            try {
//                return await continuation(request, context);
//            } catch (Polly.CircuitBreaker.BrokenCircuitException e) {
//                _logger.LogWarning($"An CircuitBreaker error occured when calling {context.Method}\n\t{e.Message}");
//                var meta = new Metadata();
//                meta.Add("service", "mysql");
//                meta.Add("message", e.Message);
//                throw new RpcException(new Status(StatusCode.DataLoss, "sql"), meta);
//            } catch (MySqlException e) {
//                _logger.LogWarning($"An SQL error occured when calling {context.Method}\n\t{e.Message}");
//                var meta = new Metadata();
//                meta.Add("service", "mysql");
//                meta.Add("message", e.Message);
//                throw new RpcException(new Status(StatusCode.DataLoss, "sql"), meta);
//            } catch (StackExchange.Redis.RedisException e) {
//                _logger.LogWarning(e, $"An REDIS error occured when calling {context.Method}\n\t{e.Message}");
//                var meta = new Metadata();
//                meta.Add("service", "redis");
//                meta.Add("message", e.Message);
//                throw new RpcException(new Status(StatusCode.DataLoss, "redis"), meta);
//            } catch (Exception e) {
//                _logger.LogError(e, $"An error occured when calling {context.Method}");
//                var meta = new Metadata();
//                meta.Add("service", "unknown");
//                meta.Add("message", e.Message);
//                throw new RpcException(new Status(StatusCode.DataLoss, "unknown"), meta);
//                //throw;
//                //throw new RpcException(Status.DefaultCancelled, e.Message);
//            }
//        }
//    }
//}
