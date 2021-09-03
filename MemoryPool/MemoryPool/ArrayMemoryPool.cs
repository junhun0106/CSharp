using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MemoryPoolBenchMarker
{
    public sealed class ArrayMemoryPool : MemoryPool<byte>
    {
        private const int MaximumBufferSize = int.MaxValue;

        private bool _isDisposed;

        private readonly Queue<ArrayMemoryPoolBuffer> _queue = new Queue<ArrayMemoryPoolBuffer>();

        public sealed override int MaxBufferSize => MaximumBufferSize;

        internal sealed class ArrayMemoryPoolBuffer : IMemoryOwner<byte>
        {
            private ArrayMemoryPool _pool;

            private byte[]? _array;

            public ArrayMemoryPoolBuffer(ArrayMemoryPool pool, int size)
            {
                _pool = pool;
                _array = ArrayPool<byte>.Shared.Rent(size);
            }

            public Memory<byte> Memory {
                get {
                    byte[]? array = _array;
                    return new Memory<byte>(array);
                }
            }

            public void Reset()
            {
                byte[]? array = _array;
                if (array != null) {
                    _array = null;
                    ArrayPool<byte>.Shared.Return(array);
                }
            }

            public void Dispose()
            {
                _pool.Return(this);
            }
        }

        public sealed override IMemoryOwner<byte> Rent(int minimumBufferSize = -1)
        {
            if (_queue.Count > 0) {
                return _queue.Dequeue();
            }

            if (minimumBufferSize == -1) {
                minimumBufferSize = 1 + (4095 / Unsafe.SizeOf<byte>());
            }

            var buffer = new ArrayMemoryPoolBuffer(this, minimumBufferSize);
            _queue.Enqueue(buffer);
            return buffer;
        }

        protected sealed override void Dispose(bool disposing)
        {
            if (_isDisposed) {
                return;
            }

            _isDisposed = true;

            while (_queue.Count > 0) {
                var buffer = _queue.Dequeue();
                buffer.Reset();
            }
        }

        internal void Return(ArrayMemoryPoolBuffer buffer)
        {
            if (!_isDisposed) {
                _queue.Enqueue(buffer);
            }
        }
    }
}
