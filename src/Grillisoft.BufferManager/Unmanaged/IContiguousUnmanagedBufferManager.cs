using System;

namespace Grillisoft.BufferManager.Unmanaged
{
    public interface IContiguousUnmanagedBufferManager : IDisposable
    {
        BufferPtr Allocate(int size);

        void Free(BufferPtr buffer);

        void Free(IntPtr buffer);

        void Init(int buffers);
    }
}