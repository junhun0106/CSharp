using System;
using System.Buffers;

namespace MemoryPoolProfiler
{
    class Program
    {
        static void Main(string[] args)
        {
            const int testCount = 100000;

            for (int i = 0; i < testCount; ++i) {
                var _ = MemoryPool<byte>.Shared.Rent(4096);
                _.Dispose();
            }

            var arrayMemoryPool = MemoryPoolFactory.CreateArrayMemoryPool();
            for (int i = 0; i < testCount; ++i) {
                var _ = arrayMemoryPool.Rent(4096);
                _.Dispose();
            }

            var slabMemoryPool = MemoryPoolFactory.CreateSlabMemoryPool();
            for (int i = 0; i < testCount; ++i) {
                var _ = slabMemoryPool.Rent(4096);
                _.Dispose();
            }

            var pinnedMemoryPool = MemoryPoolFactory.CreatePinnedBlockMemoryPool();
            for (int i = 0; i < testCount; ++i) {
                var _ = pinnedMemoryPool.Rent(4096);
                _.Dispose();
            }
        }
    }
}
