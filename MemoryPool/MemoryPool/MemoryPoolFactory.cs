// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Buffers
{
    public static class MemoryPoolFactory
    {
        public static MemoryPool<byte> CreateSlabMemoryPool() => new SlabMemoryPool();
        public static MemoryPool<byte> CreatePinnedBlockMemoryPool() => new PinnedBlockMemoryPool();
        public static MemoryPool<byte> CreateArrayMemoryPool() => new MemoryPoolBenchMarker.ArrayMemoryPool();
    }
}
