using System;

namespace Grillisoft.BufferManager.Collections
{
    internal class StandardInternal<T>
    {
        private readonly int _bufferSize;

        /// <summary>
        /// Container for the buffers in use
        /// </summary>
        private readonly BuffersHashSet<T> _buffers;

        /// <summary>
        /// Container for the cached buffers (available to be reused)
        /// </summary>
        private readonly BuffersStack<T> _cache;

        private readonly IAllocator<T> _allocator;

        private readonly bool _clear;

        public StandardInternal(IAllocator<T> allocator, bool clear, int bufferSize, int cacheSize, IAllocEvents allocEvents, IAllocEvents cacheEvents)
        {
            if (bufferSize <= 0)
                throw new ArgumentException("Buffer size must be bigger than 0", nameof(bufferSize));

            _allocator = allocator;
            _clear = clear;
            _bufferSize = bufferSize;
            _buffers = new BuffersHashSet<T>(bufferSize, allocEvents);
            _cache = new BuffersStack<T>(bufferSize, cacheEvents, cacheSize);
        }

        public void Init(int buffers)
        {
            while (buffers-- > 0)
            {
                if (!_cache.TryPush(_allocator.Allocate(_bufferSize)))
                    return;
            }
        }

        /// <summary>
        /// Allocates and return the arrays for a total of <paramref name="size"/> <see cref="T"/> elements
        /// </summary>
        /// <param name="size">The total size of the arrays to return</param>
        /// <returns></returns>
        public T[] Allocate(int size)
        {
            if (size <= 0)
                return new T[0];

            var ret = new T[((size - 1) / _bufferSize) + 1];

            for (int i = 0; i < ret.Length; i++)
                ret[i] = this.GetBuffer();

            return ret;
        }

        public void Free(T[] data)
        {
            foreach (var d in data)
                this.FreeInternal(d);
        }

        public void Free(T data)
        {
            this.FreeInternal(data);
        }

        private void FreeInternal(T data)
        {
            if (!_buffers.Remove(data))
                return;

            if (!_cache.TryPush(data))
                _allocator.Free(data);
        }

        private T GetBuffer()
        {
            return _buffers.Add(AllocateBuffer());
        }

        private T AllocateBuffer()
        {
            if (!_cache.TryPop(out var buffer))
                return _allocator.Allocate(_bufferSize);

            if (_clear)
                _allocator.Clear(buffer, _bufferSize);

            return buffer;
        }

        public void Dispose()
        {
            _buffers.Clear(_allocator.Free);
            _cache.Clear(_allocator.Free);
        }
    }
}
