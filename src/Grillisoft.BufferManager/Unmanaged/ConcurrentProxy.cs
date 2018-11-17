using System;

namespace Grillisoft.BufferManager.Unmanaged
{
    public class ConcurrentProxy : IUnmanagedBufferManager
    {
        private readonly IUnmanagedBufferManager _bufferManager;

        public ConcurrentProxy(IUnmanagedBufferManager bufferManager)
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

        /// <summary>
        /// Allocates and return the arrays for a total of <paramref name="size"/> <see cref="T"/> elements
        /// </summary>
        /// <param name="size">The total size of the arrays to return</param>
        /// <returns></returns>
        public BufferPtr[] Allocate(int size)
        {
            lock (_bufferManager)
            {
                return _bufferManager.Allocate(size);
            }
        }

        public BufferPtr AllocateSingle(int suggestedSize)
        {
            lock (_bufferManager)
            {
                return _bufferManager.AllocateSingle(suggestedSize);
            }
        }

        public void Free(IntPtr[] data)
        {
            lock (_bufferManager)
            {
                _bufferManager.Free(data);
            }
        }

        public void Free(IntPtr data)
        {
            lock (_bufferManager)
            {
                _bufferManager.Free(data);
            }
        }

        public void Dispose()
        {
            lock (_bufferManager)
            {
                _bufferManager.Dispose();
            }
        }

        public void Free(BufferPtr[] buffer)
        {
            lock (_bufferManager)
            {
                _bufferManager.Free(buffer);
            }
        }

        public void Free(BufferPtr buffer)
        {
            lock (_bufferManager)
            {
                _bufferManager.Free(buffer);
            }
        }
    }
}
