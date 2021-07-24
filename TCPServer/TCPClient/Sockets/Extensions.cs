using System.Collections.Generic;

namespace ClientLib.Chat.Sockets {
    public static class Extensions
    {
        public static int? IndexOf(this byte[] bytes, int offset, int count, byte pattern)
        {
            for (int i = offset; i < count; ++i) {
                if (bytes[i] == pattern) {
                    return i;
                }
            }
            return null;
        }

        public static void CopyTo(this byte[] bytes, int offset, int count, List<byte> dest)
        {
            if (dest == null) {
                throw new System.ArgumentNullException(nameof(dest));
            }

            for (int i = offset; i < count; ++i) {
                var b = bytes[i];
                dest.Add(b);
            }
        }
    }
}
