using System;
using System.Runtime.InteropServices;

namespace Grillisoft.BufferManager
{
    public class StandardUnmanagedBufferManager : IUnmanagedBufferManager
    {
        public const int DefaultBufferSize = 4096;
        public const int DefaultCacheSize = 1024 * 1024 * 64; //64MB

        private readonly int _bufferSize;

        /// <summary>
        /// Container for the buffers in use
        /// </summary>
        private readonly BuffersHashSet<IntPtr> _buffers;

        /// <summary>
        /// Container for the cached buffers (available to be reused)
        /// </summary>
        private readonly BuffersCache<IntPtr> _cache;

        /// <summary>
        /// Oject used to syncronise access to the BufferManager
        /// </summary>
        private readonly object _sync = new object();

        private readonly bool _clear;

        public StandardUnmanagedBufferManager(bool clear = true, int bufferSize = DefaultBufferSize, IAllocEvents allocEvents = null, ICacheEvents cacheEvents = null, int cacheSize = DefaultCacheSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentException("Buffer size must be bigger than 0", nameof(bufferSize));

            _bufferSize = bufferSize;
            _buffers = new BuffersHashSet<IntPtr>(bufferSize, allocEvents);
            _cache = new BuffersCache<IntPtr>(bufferSize, cacheEvents, cacheSize);
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
        public IntPtr[] Allocate(int size)
        {
            if (size <= 0)
                return new IntPtr[0];

            var ret = new IntPtr[((size - 1) / _bufferSize) + 1];

            for (int i = 0; i < ret.Length; i++)
                ret[i] = this.GetBuffer();

            return ret;
        }

        public void Free(IntPtr[] data)
        {
            lock (_sync)
            {
                foreach (var d in data)
                    this.FreeInternal(d);
            }
        }

        public void Free(IntPtr data)
        {
            lock (_sync)
            {
                this.FreeInternal(data);
            }
        }

        private void FreeInternal(IntPtr data)
        {
            if (!_buffers.Remove(data))
                return;

            if (!_cache.TryPush(data))
                Marshal.FreeHGlobal(data);
        }

        private IntPtr GetBuffer()
        {
            lock (_sync)
            {
                return _buffers.Add(GetFreeBuffer() ?? CreateBuffer());
            }
        }

        private IntPtr? GetFreeBuffer()
        {
            if (!_cache.TryPop(out var ret))
                return null;

            if (_clear)
                throw new NotImplementedException("Clear not implemented yet. Need to find a good memset for c#");

            return ret;
        }

        private IntPtr CreateBuffer()
        {
            return Marshal.AllocHGlobal(_bufferSize);
        }

        public void Dispose()
        {
            lock(_sync)
            {
                _buffers.Clear(ptr => Marshal.FreeHGlobal(ptr));
                _cache.Clear(ptr => Marshal.FreeHGlobal(ptr));
            }
        }
    }
}
