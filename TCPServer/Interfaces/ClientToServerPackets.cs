using System;
using System.Collections.Generic;
using System.Reflection;

namespace Interfaces
{
    /// <summary>
    /// 클라이언트 -> 서버
    /// </summary>
    public static class ClientToServerPackets {
        private static readonly Dictionary<string, Type> _packets = new Dictionary<string, Type>(StringComparer.Ordinal);
        private static readonly Dictionary<Type, string> _reversPackets = new Dictionary<Type, string>();

        public static IReadOnlyDictionary<string, Type> Packets => _packets;
        public static IReadOnlyDictionary<Type, string> ReversePackets => _reversPackets;

        public static int Count => Packets.Count;

        /// <summary>
        /// Thread Safe하지 않으므로, 반드시 MainThread 초기화 해야 한다
        /// </summary>
        static ClientToServerPackets()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types) {
                if (!type.IsClass) {
                    continue;
                }

                var attr = type.GetCustomAttribute<PacketServerAttribute>();
                if (attr != null) {
                    Add(attr.Name, type);
                }
            }
        }

        public static void Add(string name, Type type)
        {
            _packets.Add(name, type);
            _reversPackets.Add(type, name);
        }

        public static string Get(Type type)
        {
            if (ReversePackets.TryGetValue(type, out var name)) {
                return name;
            }
            return null;
        }

        public static Type Get(string name)
        {
            if (Packets.TryGetValue(name, out var type)) {
                return type;
            }

            return null;
        }
    }
}
