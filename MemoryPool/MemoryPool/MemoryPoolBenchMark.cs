
using BenchmarkDotNet.Attributes;
using System.Buffers;

namespace MemoryPoolBenchMarker
{
    [MemoryDiagnoser]
    public class MemoryPoolRentBenchMark
    {
        private const int testCount = 10;
        private const int sizeHint = 4096;

        private MemoryPool<byte> slabMemoryPool;
        private MemoryPool<byte> pinnedBlockMemoryPool;
        private MemoryPool<byte> customArrayMemoryPool;

        [GlobalSetup]
        public void Init()
        {
            slabMemoryPool = MemoryPoolFactory.CreateSlabMemoryPool();
            pinnedBlockMemoryPool = MemoryPoolFactory.CreatePinnedBlockMemoryPool();
            customArrayMemoryPool = MemoryPoolFactory.CreateArrayMemoryPool();
        }

        [Benchmark]
        public void ArrayMemoryPool()
        {
            for (int i = 0; i < testCount; ++i) {
                var memory = DefaultMemoryPool.Rent(sizeHint);
                memory.Dispose();
            }
        }

        [Benchmark]
        public void CustomArrayMemoryPool()
        {
            for (int i = 0; i < testCount; ++i) {
                var memory = customArrayMemoryPool.Rent(sizeHint);
                memory.Dispose();
            }
        }

        [Benchmark]
        public void SlabMemoryPool()
        {
            for (int i = 0; i < testCount; ++i) {
                var memory = slabMemoryPool.Rent(sizeHint);
                memory.Dispose();
            }
        }

        [Benchmark]
        public void PinnedMemoryPool()
        {
            for (int i = 0; i < testCount; ++i) {
                var memory = pinnedBlockMemoryPool.Rent(sizeHint);
                memory.Dispose();
            }
        }
    }
}
