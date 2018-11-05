using System;
using Grillisoft.BufferManager.Collections;

namespace Grillisoft.BufferManager.Unmanaged
{
    public class Standard : IUnmanagedBufferManager
    {
        public const int DefaultBufferSize = 4096;
        public const int DefaultCacheSize = 1024 * 1024 * 64; //64MB

        private readonly StandardInternal<IntPtr> _standard;

        public Standard(bool clear = true, IAllocEvents allocEvents = null, IAllocEvents cacheEvents = null, int cacheSize = DefaultCacheSize)
            : this(clear, GetBufferSize(), allocEvents, cacheEvents, cacheSize)
        {
        }

        private static int GetBufferSize()
        {
            return Environment.SystemPageSize;
        }

        public Standard(bool clear = true, int bufferSize = DefaultBufferSize, IAllocEvents allocEvents = null, IAllocEvents cacheEvents = null, int cacheSize = DefaultCacheSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentException("Buffer size must be bigger than 0", nameof(bufferSize));

            _standard = new StandardInternal<IntPtr>(new UnmanagedAllocator(), clear, bufferSize, cacheSize, allocEvents, cacheEvents);
        }

        public void Init(int buffers)
        {
            _standard.Init(buffers);
        }

        /// <summary>
        /// Allocates and return the arrays for a total of <paramref name="size"/> <see cref="T"/> elements
        /// </summary>
        /// <param name="size">The total size of the arrays to return</param>
        /// <returns></returns>
        public IntPtr[] Allocate(int size)
        {
            return _standard.Allocate(size);
        }

        public void Free(IntPtr[] data)
        {
            _standard.Free(data);
        }

        public void Free(IntPtr data)
        {
            _standard.Free(data);
        }

        public void Dispose()
        {
            _standard.Dispose();
        }
    }
}
