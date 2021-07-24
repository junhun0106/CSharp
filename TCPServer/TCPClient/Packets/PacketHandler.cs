using System;
using System.Collections.Generic;
using System.Reflection;
using Interfaces;

namespace ClientLib.Chat.Packets {
    internal sealed class PacketHandler : IDisposable
    {
        private readonly Dictionary<string, MethodInfo> handlerList = new Dictionary<string, MethodInfo>(StringComparer.Ordinal);
        private readonly HashSet<Type> _registedTypeSet = new HashSet<Type>();

        internal bool AddHandler(Type handlerClass, string handlerFuncName)
        {
            if (_registedTypeSet.Contains(handlerClass)) {
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

                var pksId = SendPackets.Get(parameters[0].ParameterType);
                if (string.IsNullOrEmpty(pksId)) {
                    continue;
                }

                handlerList.Add(pksId, method);
            }
            return true;
        }

        internal void DispatchPacket(object caller, string id, PacketClientBase pks, Action<string> errorLog = null)
        {
            if (!handlerList.TryGetValue(id, out MethodInfo info)) {
                return;
            }

            var parameters = new object[] { pks };

            try {
                info.Invoke(caller, parameters);
            } catch (Exception e) {
                errorLog?.Invoke($"{e}");
            }
        }

        public void Dispose()
        {
            handlerList.Clear();
            _registedTypeSet.Clear();
        }
    }
}
