using System;

namespace Grillisoft.BufferManager.Managed
{
    public class ConcurrentProxy<T> : IBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        private readonly IBufferManager<T> _bufferManager;

        public ConcurrentProxy(IBufferManager<T> bufferManager)
        {
            _bufferManager = bufferManager;
        }

        public void Init(int buffers)
        {
            lock (_bufferManager)
            {
                _bufferManager.Init(buffers);
            }
        }

        public T[][] Allocate(int size)
        {
            lock (_bufferManager)
            {
                return _bufferManager.Allocate(size);
            }
        }

        public void Free(T[][] data)
        {
            lock (_bufferManager)
            {
                _bufferManager.Free(data);
            }
        }

        public void Free(T[] data)
        {
            lock (_bufferManager)
            {
                _bufferManager.Free(data);
            }
        }
    }
}
