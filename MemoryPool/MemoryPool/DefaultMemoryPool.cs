using System.Buffers;

namespace MemoryPoolBenchMarker
{
    public static class DefaultMemoryPool
    {
        private static readonly MemoryPool<byte> _shared = MemoryPool<byte>.Shared;

        public static IMemoryOwner<byte> Rent(int bufferSize = -1) => _shared.Rent(bufferSize);
    }
}
