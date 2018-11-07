using System;
using System.Linq;

namespace Grillisoft.BufferManager.Managed
{
    public class Multi<T> : IBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        private readonly int[] _bufferSizes;
        private readonly Standard<T>[] _managers;

        public Multi(int[] bufferSizes, int[] cacheSizes, bool clear = true, IAllocEvents allocEvents = null, IAllocEvents cacheEvents = null)
        {
            if (bufferSizes == null || bufferSizes.Length <= 0)
                throw new ArgumentException("Buffer sizes array is null or empty", nameof(bufferSizes));

            if (cacheSizes == null || cacheSizes.Length <= 0)
                throw new ArgumentException("Cache sizes array is null or empty", nameof(bufferSizes));

            if (bufferSizes.Length != cacheSizes.Length)
                throw new ArgumentException("Cache sizes array length must match Buffer sizes array length", nameof(cacheSizes));

            if (bufferSizes.Distinct().Count() != bufferSizes.Length)
                throw new ArgumentException("Duplicated Buffer sizes are not allowed", nameof(bufferSizes));

            _managers = Enumerable.Range(0, bufferSizes.Length - 1)
                                  .Select(i => new Standard<T>(clear, bufferSizes[i], allocEvents, cacheEvents, cacheSizes[i]))
                                  .OrderBy(m => m.BufferSize)
                                  .ToArray();

            _bufferSizes = _managers.Select(m => m.BufferSize).ToArray();
        }

        private int GetIndex(int size)
        {
            for (int i = 0; i < _bufferSizes.Length; i++)
                if (size <= _bufferSizes[i])
                    return i;

            return _bufferSizes.Length - 1;
        }

        public T[][] Allocate(int size)
        {
            return _managers[GetIndex(size)].Allocate(size);
        }

        public void Free(T[][] data)
        {
            _managers[GetIndex(data.First().Length)].Free(data);
        }

        public void Free(T[] data)
        {
            _managers[GetIndex(data.Length)].Free(data);
        }

        public void Init(int buffers)
        {
            foreach (var manager in _managers)
                manager.Init(buffers);
        }
    }
}
