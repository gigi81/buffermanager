using System;
using Grillisoft.BufferManager.Collections;

namespace Grillisoft.BufferManager.Managed
{
    public class Standard<T> : IBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        public const int DefaultBufferSize = 4096;
        public const int DefaultCacheSize = 1024 * 1024 * 64; //64MB

        private readonly StandardInternal<T[]> _standard;

        public Standard(bool clear = true, IAllocEvents allocEvents = null, IAllocEvents cacheEvents = null, int cacheSize = DefaultCacheSize)
            : this(clear, GetBufferSize<T>(), allocEvents, cacheEvents, cacheSize)
        {
        }

        private static int GetBufferSize<TItem>() where TItem : struct
        {
            return Environment.SystemPageSize / System.Runtime.InteropServices.Marshal.SizeOf(typeof(TItem));
        }

        public Standard(bool clear = true, int bufferSize = DefaultBufferSize, IAllocEvents allocEvents = null, IAllocEvents cacheEvents = null, int cacheSize = DefaultCacheSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentException("Buffer size must be bigger than 0", nameof(bufferSize));

            _standard = new StandardInternal<T[]>(new ManagedAllocator<T>(), clear, bufferSize, cacheSize, allocEvents, cacheEvents);
        }

        public int BufferSize => _standard.BufferSize;

        public void Init(int buffers)
        {
            _standard.Init(buffers);
        }

        public T[][] Allocate(int size)
        {
            return _standard.Allocate(size);
        }

        public void Free(T[][] data)
        {
            _standard.Free(data);
        }

        public void Free(T[] data)
        {
            _standard.Free(data);
        }
    }
}
