using System;
using System.Collections.Generic;
using System.Reflection;
using Interfaces;
using Microsoft.Extensions.Logging;

namespace ChatService.Packets {
    public interface IDispatcher
    {
        void DispatchPacket(object caller, string id, PacketServerBase pks);
    }

    public class PacketHandler : IDispatcher
    {
        private readonly Dictionary<string, MethodInfo> handlerList = new Dictionary<string, MethodInfo>(StringComparer.Ordinal);
        private readonly HashSet<Type> _registedTypeSet = new HashSet<Type>();
        private readonly ILogger _logger;

        public PacketHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PacketHandler>();
        }

        internal bool AddHandler(Type handlerClass, string handlerFuncName)
        {
            if (_registedTypeSet.Contains(handlerClass)) {
                _logger.LogWarning("AddHandler - already registered. {0}", handlerClass.Name);
                return false;
            }

            _registedTypeSet.Add(handlerClass);

            var methods = new List<MethodInfo>();
            foreach (var method in handlerClass.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic)) {
                if (method.Name == handlerFuncName) {
                    methods.Add(method);
                }
            }

            foreach (var method in methods) {
                var parameters = method.GetParameters();
                if (parameters.Length != 1) {
                    continue;
                }

                var pksId = ReceivePackets.Get(parameters[0].ParameterType);
                if (string.IsNullOrEmpty(pksId)) {
                    _logger.LogWarning("AddHandler - pksId is empty");
                    continue;
                }

                handlerList.Add(pksId, method);
            }

            return true;
        }

        public void DispatchPacket(object caller, string id, PacketServerBase pks)
        {
            if (!handlerList.TryGetValue(id, out MethodInfo info)) {
                _logger.LogWarning("DispatchPacket - not found id : {0}", id);
                return;
            }

            var parameters = new object[] { pks };

            try {
                info.Invoke(caller, parameters);
            } catch (Exception e) {
                _logger.LogError($"DispatchPacket - {e}");
            }
        }
    }
}
