using System;

namespace Grillisoft.BufferManager
{
    public class StandardBufferManager<T> : IBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        public const int DefaultBufferSize = 4096;
        public const int DefaultCacheSize = 1024 * 1024 * 64; //64MB

        private readonly int _bufferSize;

        /// <summary>
        /// Container for the buffers in use
        /// </summary>
        private readonly BuffersHashSet<T[]> _buffers;

        /// <summary>
        /// Container for the cached buffers (available to be reused)
        /// </summary>
        private readonly BuffersCache<T[]> _cache;

        /// <summary>
        /// Oject used to syncronise access to the BufferManager
        /// </summary>
        private readonly object _sync = new object();

        private readonly bool _clear;

        public StandardBufferManager(bool clear = true, int bufferSize = DefaultBufferSize, IAllocEvents allocEvents = null, ICacheEvents cacheEvents = null, int cacheSize = DefaultCacheSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentException("Buffer size must be bigger than 0", nameof(bufferSize));

            _bufferSize = bufferSize;
            _buffers = new BuffersHashSet<T[]>(bufferSize, allocEvents);
            _cache = new BuffersCache<T[]>(bufferSize, cacheEvents, cacheSize);
            _clear = clear;
        }

        public void Init(int buffers)
        {
            lock (_sync)
            {
                while (buffers-- > 0)
                {
                    if (!_cache.TryPush(this.CreateBuffer()))
                        return;
                }
            }
        }

        /// <summary>
        /// Allocates and return the arrays for a total of <paramref name="size"/> <see cref="T"/> elements
        /// </summary>
        /// <param name="size">The total size of the arrays to return</param>
        /// <returns></returns>
        public T[][] Allocate(int size)
        {
            if (size <= 0)
                return new T[0][];

            var ret = new T[((size - 1) / _bufferSize) + 1][];

            for (int i = 0; i < ret.Length; i++)
                ret[i] = this.GetBuffer();

            return ret;
        }

        public void Free(T[][] data)
        {
            lock (_sync)
            {
                foreach (var d in data)
                    this.FreeInternal(d);
            }
        }

        public void Free(T[] data)
        {
            lock (_sync)
            {
                this.FreeInternal(data);
            }
        }

        private void FreeInternal(T[] data)
        {
            if (!_buffers.Remove(data))
                return;

            _cache.TryPush(data);
        }

        private T[] GetBuffer()
        {
            lock (_sync)
            {
                return _buffers.Add(GetFreeBuffer() ?? CreateBuffer());
            }
        }

        private T[] GetFreeBuffer()
        {
            if (!_cache.TryPop(out var ret))
                return null;

            if (_clear)
                Array.Clear(ret, 0, ret.Length);

            return ret;
        }

        private T[] CreateBuffer()
        {
            return new T[_bufferSize];
        }
    }
}
